namespace Kirnau.Survey.Admin.Areas.Survey.Extension
{
    using System.Collections.Specialized;
    using System.Web.Routing;

    internal static class RouteValueDictionaryExtensions
    {
        public static void MergeQueryToRouteValues(this RouteValueDictionary routeValues, NameValueCollection queryValues)
        {
            foreach (string key in queryValues.AllKeys)
            {
                routeValues[key] = queryValues[key];
            }
        }
    }
}