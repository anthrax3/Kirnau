using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Web.Helpers;
using System.Web.Routing;
using Microsoft.IdentityModel.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Web;
using Microsoft.IdentityModel.Web.Configuration;
using Microsoft.Practices.Unity;

using Kirnau.Survey.Admin.Controllers;

namespace Kirnau.Survey.Admin
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var container = new UnityContainer();
            ContainerBootstraper.RegisterTypes(container, false);
            ControllerBuilder.Current.SetControllerFactory(new UnityControllerFactory(container));

            //RouteTable.Routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            //AreaRegistration.RegisterAllAreas();
            //AppRoutes.RegisterRoutes(RouteTable.Routes);

            FederatedAuthentication.ServiceConfigurationCreated += OnServiceConfigurationCreated;

            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.Name;
        }
        private static void OnServiceConfigurationCreated(object sender, ServiceConfigurationCreatedEventArgs e)
        {
            // Use the <serviceCertificate> to protect the cookies that are
            // sent to the client.
            var sessionTransforms =
                new List<CookieTransform>(
                    new CookieTransform[] 
                    {
                        new DeflateCookieTransform(), 
                        new RsaEncryptionCookieTransform(e.ServiceConfiguration.ServiceCertificate),
                        new RsaSignatureCookieTransform(e.ServiceConfiguration.ServiceCertificate)  
                    });
            var sessionHandler = new SessionSecurityTokenHandler(sessionTransforms.AsReadOnly());
            e.ServiceConfiguration.SecurityTokenHandlers.AddOrReplace(sessionHandler);
        }

        private void Application_Error(object sender, System.EventArgs e)
        {
            System.Exception ex = Server.GetLastError();

            if (ex is System.Web.HttpRequestValidationException)
            {
            }
        }
    }
}
