namespace Kirnau.Survey.Admin.Models
{
    using Kirnau.Survey.Web.Shared.Models;

    public class TenantMasterPageViewData
    {
        public string Title { get; set; }

        public Tenant Tenant { get; set; }
    }
}