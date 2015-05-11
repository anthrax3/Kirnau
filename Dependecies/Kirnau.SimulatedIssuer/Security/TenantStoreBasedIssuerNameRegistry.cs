namespace Kirnau.SimulatedIssuer.Security
{
    using System.IdentityModel.Tokens;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.Practices.Unity;
    using Kirnau.Survey.Web.Shared.Stores;

    public class TenantStoreBasedIssuerNameRegistry : Microsoft.IdentityModel.Tokens.IssuerNameRegistry
    {
        private readonly ITenantStore tenantStore;

        public TenantStoreBasedIssuerNameRegistry()
        {
            using (var container = new UnityContainer())
            {
                ContainerBootstraper.RegisterTypes(container);
                this.tenantStore = container.Resolve<ITenantStore>();
            }
        }

        public override string GetIssuerName(SecurityToken securityToken)
        {
            if (securityToken is X509SecurityToken)
            {
                string thumbprint = (securityToken as X509SecurityToken).Certificate.Thumbprint;
                foreach (var tenantName in this.tenantStore.GetTenantNames())
                {
                    var tenant = this.tenantStore.GetTenant(tenantName);
                    if (tenant.IssuerThumbPrint.Equals(thumbprint, System.StringComparison.InvariantCultureIgnoreCase))
                    {
                        return tenant.Name;
                    }
                }
                return null;
            }
            else
            {
                throw new InvalidSecurityTokenException("Empty or wrong securityToken argument");
            }
        }
    }
}