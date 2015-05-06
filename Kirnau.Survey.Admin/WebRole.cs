namespace Kirnau.Survey.Admin
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using System.Security.Permissions;
    using Microsoft.Practices.Unity;
    using Microsoft.WindowsAzure.ServiceRuntime;
    using Kirnau.Survey.Web.Shared.Stores;
    using Kirnau.Survey.Admin._sampleInitializationResources;
    using Kirnau.Survey.Web.Shared.Helpers;

    public class WebRole : RoleEntryPoint
    {
        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Justification = "This is the default code from the project template for the Windows Azure SDK.")]
        public override bool OnStart()
        {
            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.
            RoleEnvironment.Changing += RoleEnvironmentChanging;
            RoleEnvironment.Changed += RoleEnvironmentChanged;

            using (var container = new UnityContainer())
            {
                ContainerBootstraper.RegisterTypes(container, true);

                container.Resolve<ISurveyStore>().Initialize();
                container.Resolve<ISurveyAnswerStore>().Initialize();
                container.Resolve<ISurveyAnswersSummaryStore>().Initialize();
                container.Resolve<ISurveyTransferStore>().Initialize();

                var tenantStore = container.Resolve<ITenantStore>();
                tenantStore.Initialize();

                // Set Adatum's logo
                SetLogo("adatum", tenantStore, TenantInitLogo.adatum_logo);

                // Set Fabrikam's logo
                SetLogo("fabrikam", tenantStore, TenantInitLogo.fabrikam_logo);

                return base.OnStart();
            }
        }

        private static void SetLogo(string tenant, ITenantStore tenantStore, Bitmap logo)
        {
            if (string.IsNullOrWhiteSpace(tenantStore.GetTenant(tenant).Logo))
            {
                using (var stream = new MemoryStream())
                {
                    logo.Save(stream, ImageFormat.Png);
                    tenantStore.UploadLogo(tenant, stream.ToArray());
                }
            }
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
    }
}
