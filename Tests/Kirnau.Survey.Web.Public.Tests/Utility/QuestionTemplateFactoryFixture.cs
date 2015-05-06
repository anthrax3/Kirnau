namespace Kirnau.Survey.Web.Public.Tests.Utility
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Kirnau.Survey.Web.Public.Utility;
    using Kirnau.Survey.Web.Shared.Models;

    [TestClass]
    public class QuestionTemplateFactoryFixture
    {
        [TestMethod]
        public void CreateForSimpleText()
        {
            Assert.AreEqual(QuestionType.SimpleText.ToString(), QuestionTemplateFactory.Create(QuestionType.SimpleText));
        }

        [TestMethod]
        public void CreateForMultipleChoice()
        {
            Assert.AreEqual(QuestionType.MultipleChoice.ToString(), QuestionTemplateFactory.Create(QuestionType.MultipleChoice));
        }

        [TestMethod]
        public void CreateForFiveStars()
        {
            Assert.AreEqual(QuestionType.FiveStars.ToString(), QuestionTemplateFactory.Create(QuestionType.FiveStars));
        }
    }
}
