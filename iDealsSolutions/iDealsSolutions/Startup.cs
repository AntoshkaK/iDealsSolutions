using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(iDealsSolutions.Startup))]
namespace iDealsSolutions
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
