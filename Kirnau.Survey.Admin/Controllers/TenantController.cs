namespace Kirnau.Survey.Admin.Controllers
{
    using System;
    using System.Globalization;
    using System.Web.Mvc;
    using Kirnau.Survey.Admin.Models;
    using Kirnau.Survey.Web.Shared.Models;
    using Kirnau.Survey.Web.Shared.Stores;

    public abstract class TenantController : AsyncController
    {
        private readonly ITenantStore tenantStore;
        private string tenantName;

        protected TenantController(ITenantStore tenantStore)
        {
            this.tenantStore = tenantStore;
        }

        public ITenantStore TenantStore
        {
            get { return this.tenantStore; }
        }

        public string TenantName
        {
            get
            {
                return this.tenantName;
            }

            set
            {
                this.tenantName = value;
                this.ViewData["tenant"] = value;
            }
        }

        public Tenant Tenant { get; set; }

        public TenantPageViewData<T> CreateTenantPageViewData<T>(T contentModel)
        {
            var tenantPageViewData = new TenantPageViewData<T>(contentModel)
            {
                Tenant = this.Tenant
            };
            return tenantPageViewData;
        }

        public TenantUdfPageViewData<T> CreateTenantUdfPageViewData<T>(string model, T contentModel)
        {
            var tenantUdfPageViewData = new TenantUdfPageViewData<T>(model, contentModel)
            {
                Tenant = this.Tenant
            };
            return tenantUdfPageViewData;
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.RouteData.Values["tenant"] != null)
            {
                this.TenantName = (string)filterContext.RouteData.Values["tenant"];
            }

            if (this.Tenant == null)
            {
                var tenant = this.TenantStore.GetTenant(this.tenantName);
                if (tenant == null)
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentUICulture, "'{0}' is not a valid tenant.", this.tenantName));
                }

                this.Tenant = tenant;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
