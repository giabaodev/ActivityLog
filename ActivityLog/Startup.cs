using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ActivityLog.Startup))]
namespace ActivityLog
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
