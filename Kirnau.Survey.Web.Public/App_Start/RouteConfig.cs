using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Kirnau.Survey.Web.Public
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            //routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //routes.MapRoute(
            //    name: "Default",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            //);
            routes.MapRoute(
                "Home",
                string.Empty,
                new { controller = "Surveys", action = "Index" });

            routes.MapRoute(
                "ViewSurvey",
                "survey/{tenant}/{surveySlug}",
                new { controller = "Surveys", action = "Display" });

            routes.MapRoute(
                "ThankYouForFillingTheSurvey",
                "survey/{tenant}/{surveySlug}/thankyou",
                new { controller = "Surveys", action = "ThankYou" });
        }
    }
}
