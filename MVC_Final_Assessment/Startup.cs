using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MVC_Final_Assessment.Startup))]
namespace MVC_Final_Assessment
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
