using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Selenium.Startup))]
namespace Selenium
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
