using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Kirnau.Survey.Web.Public.Startup))]
namespace Kirnau.Survey.Web.Public
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
