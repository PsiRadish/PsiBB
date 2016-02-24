using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PsiBB.Startup))]
namespace PsiBB
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
