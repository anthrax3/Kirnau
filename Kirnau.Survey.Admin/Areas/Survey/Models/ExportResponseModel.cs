namespace Kirnau.Survey.Admin.Areas.Survey.Models
{
    using Kirnau.Survey.Web.Shared.Models;

    public class ExportResponseModel
    {
        public bool HasResponses { get; set; }

        public Tenant Tenant { get; set; }
    }
}