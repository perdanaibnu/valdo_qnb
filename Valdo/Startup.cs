using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Valdo.Startup))]
namespace Valdo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
