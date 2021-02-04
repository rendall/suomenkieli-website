using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SuomenkieliWebsite.Startup))]
namespace SuomenkieliWebsite
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
