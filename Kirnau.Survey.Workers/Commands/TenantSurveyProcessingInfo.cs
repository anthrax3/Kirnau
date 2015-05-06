namespace Kirnau.Survey.Workers.Commands
{
    using System.Collections.Generic;
    using Kirnau.Survey.Web.Shared.Models;
    using Kirnau.Survey.Web.Shared.QueueMessages;

    public class TenantSurveyProcessingInfo
    {
        public TenantSurveyProcessingInfo(string tenant, string slugName)
        {
            this.AnswersSummary = new SurveyAnswersSummary(tenant, slugName);
            this.AnswersMessages = new List<SurveyAnswerStoredMessage>();
        }

        public SurveyAnswersSummary AnswersSummary { get; private set; }

        public IList<SurveyAnswerStoredMessage> AnswersMessages { get; private set; }
    }
}
