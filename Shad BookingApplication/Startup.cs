using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Shad_BookingApplication.Startup))]
namespace Shad_BookingApplication
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
