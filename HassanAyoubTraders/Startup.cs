using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(HassanAyoubTraders.Startup))]
namespace HassanAyoubTraders
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
