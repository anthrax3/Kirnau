namespace Kirnau.Survey.Workers
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Net;
    using System.Security.Permissions;
    using System.Threading;
    using Microsoft.Practices.Unity;

    using Kirnau.Survey.Web.Shared.Helpers;
    using Kirnau.Survey.Web.Shared.Models;
    using Kirnau.Survey.Web.Shared.Stores;
    using Kirnau.Survey.Workers.Commands;
    using Kirnau.Survey.Workers.QueueHandlers;
    using Survey.Web.Shared.QueueMessages;
    using Survey.Web.Shared.Stores.AzureStorage;
    using Microsoft.WindowsAzure.ServiceRuntime;

    public sealed class WorkerRole : RoleEntryPoint, IDisposable
    {
        private IUnityContainer container;

        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Justification = "This is the default code from the project template for the Windows Azure SDK.")]
        public override bool OnStart()
        {
            // The number of connections depends on the particular usage in each application
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.
            RoleEnvironment.Changing += RoleEnvironmentChanging;
            RoleEnvironment.Changed += RoleEnvironmentChanged;

            this.container = new UnityContainer();
            ContainerBootstraper.RegisterTypes(this.container, true);

            this.container.Resolve<ISurveyAnswerStore>().Initialize();
            this.container.Resolve<ISurveyAnswersSummaryStore>().Initialize();
            this.container.Resolve<ITenantStore>().Initialize();
            this.container.Resolve<ISurveyStore>().Initialize();
            this.container.Resolve<ISurveyTransferStore>().Initialize();

            return base.OnStart();
        }

        public override void Run()
        {
            //// The time interval for checking the queues have to be tuned depending on the scenario and the expected workload
            var standardQueue = this.container.Resolve<IAzureQueue<SurveyAnswerStoredMessage>>(SubscriptionKind.Standard.ToString());
            var premiumQueue = this.container.Resolve<IAzureQueue<SurveyAnswerStoredMessage>>(SubscriptionKind.Premium.ToString());

            BatchMultipleQueueHandler
                .For(premiumQueue, GetPremiumQueueBatchSize())
                .AndFor(standardQueue, GetStandardQueueBatchSize())
                .Every(TimeSpan.FromSeconds(GetSummaryUpdatePollingInterval()))
                .WithLessThanTheseBatchIterationsPerCycle(GetMaxBatchIterationsPerCycle())
                .Do(this.container.Resolve<UpdatingSurveyResultsSummaryCommand>());

            QueueHandler
                .For(this.container.Resolve<IAzureQueue<SurveyTransferMessage>>())
                .Every(TimeSpan.FromSeconds(GetExportRequestPollingInterval()))
                .Do(this.container.Resolve<TransferSurveysToSqlAzureCommand>());

            while (true)
            {
                Thread.Sleep(TimeSpan.FromSeconds(5));
            }
        }

        public void Dispose()
        {
            this.container.Dispose();
        }

        private static void RoleEnvironmentChanging(object sender, RoleEnvironmentChangingEventArgs e)
        {
            // for any configuration setting change except TraceEventTypeFilter
            if (e.Changes.OfType<RoleEnvironmentConfigurationSettingChange>().Any(change => change.ConfigurationSettingName != "TraceEventTypeFilter"))
            {
                // Set e.Cancel to true to restart this role instance
                e.Cancel = true;
            }
        }

        private static void RoleEnvironmentChanged(object sender, RoleEnvironmentChangedEventArgs e)
        {
            // configure trace listener for any changes to EnableTableStorageTraceListener 
            if (e.Changes.OfType<RoleEnvironmentConfigurationSettingChange>().Any(change => change.ConfigurationSettingName == "TraceEventTypeFilter"))
            {
                ConfigureTraceListener(RoleEnvironment.GetConfigurationSettingValue("TraceEventTypeFilter"));
            }
        }

        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        private static void ConfigureTraceListener(string traceEventTypeFilter)
        {
            SourceLevels sourceLevels;
            if (Enum.TryParse(traceEventTypeFilter, true, out sourceLevels))
            {
                TraceHelper.Configure(sourceLevels);
            }
        }

        private static int GetPremiumQueueBatchSize()
        {
            try
            {
                return Math.Max(1, Convert.ToInt32(CloudConfiguration.GetConfigurationSetting("PremiumBatchProcessingSize")));
            }
            catch
            {
                TraceHelper.TraceWarning("Bad parameter value: PremiumBatchProcessingSize");
                return 32;
            }
        }

        private static int GetStandardQueueBatchSize()
        {
            try
            {
                return Math.Max(1, Convert.ToInt32(CloudConfiguration.GetConfigurationSetting("StandardBatchProcessingSize")));
            }
            catch
            {
                TraceHelper.TraceWarning("Bad parameter value: StandardBatchProcessingSize");
                return 8;
            }
        }

        private static int GetSummaryUpdatePollingInterval()
        {
            try
            {
                return Math.Max(5, Convert.ToInt32(CloudConfiguration.GetConfigurationSetting("SummaryUpdatePollingInterval")));
            }
            catch
            {
                TraceHelper.TraceWarning("Bad parameter value: SummaryUpdatePollingInterval");
                return 10;
            }
        }

        private static int GetExportRequestPollingInterval()
        {
            try
            {
                return Math.Max(5, Convert.ToInt32(CloudConfiguration.GetConfigurationSetting("ExportRequestPollingInterval")));
            }
            catch
            {
                TraceHelper.TraceWarning("Bad parameter value: ExportRequestPollingInterval");
                return 10;
            }
        }

        private static int GetMaxBatchIterationsPerCycle()
        {
            try
            {
                return Math.Max(2, Convert.ToInt32(CloudConfiguration.GetConfigurationSetting("MaxBatchIterationsPerCycle")));
            }
            catch
            {
                TraceHelper.TraceWarning("Bad parameter value: MaxBatchIterationsPerCycle");
                return 4;
            }
        }
    }
}
