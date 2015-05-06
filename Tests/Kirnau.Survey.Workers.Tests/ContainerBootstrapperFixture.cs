namespace Kirnau.Survey.Workers.Tests
{
    using Microsoft.Practices.Unity;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Kirnau.Survey.Workers;
    using Survey.Web.Shared.Stores;

    [TestClass]
    public class ContainerBootstrapperFixture
    {
        [TestMethod]
        public void ResolveISurveyAnswerStore()
        {
            using (var container = new UnityContainer())
            {
                ContainerBootstraper.RegisterTypes(container);
                var actualObject = container.Resolve<ISurveyAnswerStore>();

                Assert.IsInstanceOfType(actualObject, typeof(SurveyAnswerStore));
            }
        }

        [TestMethod]
        public void ResolveISurveyAnswersSummaryStore()
        {
            using (var container = new UnityContainer())
            {
                ContainerBootstraper.RegisterTypes(container);
                var actualObject = container.Resolve<ISurveyAnswersSummaryStore>();

                Assert.IsInstanceOfType(actualObject, typeof(SurveyAnswersSummaryStore));
            }
        }
    }
}
