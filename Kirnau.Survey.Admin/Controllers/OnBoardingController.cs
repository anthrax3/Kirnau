namespace Kirnau.Survey.Admin.Controllers
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Kirnau.Survey.Admin.Models;
    using Kirnau.Survey.Web.Shared.Models;
    using Kirnau.Survey.Web.Shared.Stores;

    public class OnBoardingController : Controller
    {
        private readonly ITenantStore tenantStore;

        public OnBoardingController(ITenantStore tenantStore)
        {
            this.tenantStore = tenantStore;
        }

        public ITenantStore TenantStore
        {
            get { return this.tenantStore; }
        }

        [HttpGet]
        public ActionResult Index()
        {
            IList<Tenant> tenants = new List<Tenant>();
            foreach (var tenantName in this.tenantStore.GetTenantNames())
            {
                tenants.Add(this.tenantStore.GetTenant(tenantName));
            }

            var model = new TenantPageViewData<IEnumerable<Tenant>>(tenants)
            {
                Title = "On boarding"
            };
            return this.View(model);
        }

        [HttpGet]
        public ActionResult Join()
        {
            var model = new TenantMasterPageViewData { Title = "Join Tailspin" };
            return this.View(model);
        }
    }
}
