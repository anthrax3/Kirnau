using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Kirnau.Survey.Admin.Startup))]
namespace Kirnau.Survey.Admin
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
