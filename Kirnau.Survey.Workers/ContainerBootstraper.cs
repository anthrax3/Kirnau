namespace Kirnau.Survey.Workers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.Practices.Unity;
    using Kirnau.Survey.Web.Shared.Helpers;
    using Kirnau.Survey.Web.Shared.QueueMessages;
    using Kirnau.Survey.Web.Shared.Stores.Azure;
    using Kirnau.Survey.Workers.Commands;
    using Kirnau.Survey.Web.Shared;
    using Survey.Web.Shared.Models;
    using Survey.Web.Shared.Stores;
    using Survey.Web.Shared.Stores.AzureStorage;

    public static class ContainerBootstraper
    {
        public static void RegisterTypes(IUnityContainer container)
        {
            RegisterTypes(container, true);
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Microsoft.DisposeObjectsBeforeLosingScope", Justification = "This container is used by the main container and cannot be disposed.")]
        public static void RegisterTypes(IUnityContainer container, bool roleInitialization)
        {
            var account = CloudConfiguration.GetStorageAccount("DataConnectionString");

            container.RegisterInstance(account);

            // http://msdn.microsoft.com/en-us/library/hh680900(v=pandp.50).aspx
            container.RegisterInstance<IRetryPolicyFactory>(roleInitialization
                ? new DefaultRetryPolicyFactory() as IRetryPolicyFactory
                : new ConfiguredRetryPolicyFactory() as IRetryPolicyFactory);

            container.RegisterType<IDictionary<string, TenantSurveyProcessingInfo>, Dictionary<string, TenantSurveyProcessingInfo>>(new InjectionConstructor());

            var cloudStorageAccountType = typeof(Microsoft.WindowsAzure.Storage.CloudStorageAccount);
            var retryPolicyFactoryProperty = new InjectionProperty("RetryPolicyFactory", typeof(IRetryPolicyFactory));

            // registering IAzureTable types
            container
                .RegisterType<IAzureTable<SurveyRow>, AzureTable<SurveyRow>>(
                    new InjectionConstructor(cloudStorageAccountType, AzureConstants.Tables.Surveys),
                    retryPolicyFactoryProperty)
                .RegisterType<IAzureTable<QuestionRow>, AzureTable<QuestionRow>>(
                    new InjectionConstructor(cloudStorageAccountType, AzureConstants.Tables.Questions),
                    retryPolicyFactoryProperty);

            // registering IAzureQueue types
            var visibilityTime = TimeSpan.FromSeconds(300);
            container
                .RegisterType<IAzureQueue<SurveyAnswerStoredMessage>, AzureQueue<SurveyAnswerStoredMessage>>(
                    SubscriptionKind.Standard.ToString(),
                    new InjectionConstructor(cloudStorageAccountType, AzureConstants.Queues.SurveyAnswerStoredStandard, visibilityTime),
                    retryPolicyFactoryProperty)
                .RegisterType<IAzureQueue<SurveyAnswerStoredMessage>, AzureQueue<SurveyAnswerStoredMessage>>(
                    SubscriptionKind.Premium.ToString(),
                    new InjectionConstructor(cloudStorageAccountType, AzureConstants.Queues.SurveyAnswerStoredPremium, visibilityTime),
                    retryPolicyFactoryProperty)
                .RegisterType<IAzureQueue<SurveyTransferMessage>, AzureQueue<SurveyTransferMessage>>(
                    new InjectionConstructor(cloudStorageAccountType, AzureConstants.Queues.SurveyTransferRequest, visibilityTime),
                    retryPolicyFactoryProperty);

            // registering IAzureBlobContainer types
            container
                .RegisterType<IAzureBlobContainer<byte[]>, FilesBlobContainer>(
                    new InjectionConstructor(cloudStorageAccountType, AzureConstants.BlobContainers.Logos, "image/jpeg"),
                    retryPolicyFactoryProperty)
                .RegisterType<IAzureBlobContainer<SurveyAnswersSummary>, EntitiesBlobContainer<SurveyAnswersSummary>>(
                    new InjectionConstructor(cloudStorageAccountType, AzureConstants.BlobContainers.SurveyAnswersSummaries),
                    retryPolicyFactoryProperty)
                .RegisterType<IAzureBlobContainer<List<string>>, EntitiesBlobContainer<List<string>>>(
                    new InjectionConstructor(cloudStorageAccountType, AzureConstants.BlobContainers.SurveyAnswersLists),
                    retryPolicyFactoryProperty)
                .RegisterType<IAzureBlobContainer<Tenant>, EntitiesBlobContainer<Tenant>>(
                    new InjectionConstructor(cloudStorageAccountType, AzureConstants.BlobContainers.Tenants),
                    retryPolicyFactoryProperty);

            var cacheEnabledProperty = new InjectionProperty("CacheEnabled", !roleInitialization && Convert.ToBoolean(CloudConfiguration.GetConfigurationSetting("EnableCaching")));

            // registering Store types
            container
                .RegisterType<ISurveyStore, SurveyStore>(cacheEnabledProperty)
                .RegisterType<ITenantStore, TenantStore>(cacheEnabledProperty)
                .RegisterType<ISurveyAnswerStore, SurveyAnswerStore>(new InjectionFactory((c, t, s) => new SurveyAnswerStore(
                    container.Resolve<ITenantStore>(),
                    container.Resolve<ISurveyAnswerContainerFactory>(),
                    container.Resolve<IAzureQueue<SurveyAnswerStoredMessage>>(SubscriptionKind.Standard.ToString()),
                    container.Resolve<IAzureQueue<SurveyAnswerStoredMessage>>(SubscriptionKind.Premium.ToString()),
                    container.Resolve<IAzureBlobContainer<List<string>>>())))
                .RegisterType<ISurveyAnswersSummaryStore, SurveyAnswersSummaryStore>()
                .RegisterType<ISurveySqlStore, SurveySqlStore>()
                .RegisterType<ISurveyTransferStore, SurveyTransferStore>();

            // Container for resolving the survey answer containers
            var surveyAnswerBlobContainerResolver = new UnityContainer();

            surveyAnswerBlobContainerResolver.RegisterInstance(account);

            // http://msdn.microsoft.com/en-us/library/hh680900(v=pandp.50).aspx
            surveyAnswerBlobContainerResolver.RegisterInstance<IRetryPolicyFactory>(roleInitialization
                ? new DefaultRetryPolicyFactory() as IRetryPolicyFactory
                : new ConfiguredRetryPolicyFactory() as IRetryPolicyFactory);

            surveyAnswerBlobContainerResolver.RegisterType<IAzureBlobContainer<SurveyAnswer>, EntitiesBlobContainer<SurveyAnswer>>(
                new InjectionConstructor(cloudStorageAccountType, typeof(string)),
                retryPolicyFactoryProperty);

            container.RegisterType<ISurveyAnswerContainerFactory, SurveyAnswerContainerFactory>(
                new InjectionConstructor(surveyAnswerBlobContainerResolver));
        }
    }
}