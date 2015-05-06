namespace Kirnau.Survey.Web.Shared.Tests.Stores
{
    using Microsoft.Practices.Unity;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.Storage;
    using Kirnau.Survey.Web.Shared.Models;
    using Kirnau.Survey.Web.Shared.Stores;
    using Kirnau.Survey.Web.Shared.Stores.AzureStorage;

    [TestClass]
    public class SurveyAnswerContainerFactoryFixture
    {
        [TestMethod]
        public void NewInstanceIsTypeOfSurveyAnswerContainerFactory()
        {
            var instance = new SurveyAnswerContainerFactory(null);
            Assert.IsInstanceOfType(instance, typeof(SurveyAnswerContainerFactory));
        }

        [TestMethod]
        public void CreateReturnsAnswersContainer()
        {
            using (var container = new UnityContainer())
            {
                var factory = new SurveyAnswerContainerFactory(container);
                container.RegisterInstance(CloudStorageAccount.DevelopmentStorageAccount);
                container.RegisterType<IAzureBlobContainer<SurveyAnswer>, EntitiesBlobContainer<SurveyAnswer>>();
                var blobContainer = factory.Create("tenant", "surveySlug");
                Assert.IsInstanceOfType(blobContainer, typeof(EntitiesBlobContainer<SurveyAnswer>));
            }
        }
    }
}
