using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Samscape.Startup))]
namespace Samscape
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
