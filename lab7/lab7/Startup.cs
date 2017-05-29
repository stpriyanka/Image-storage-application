using lab7.Models;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(lab7.Startup))]
namespace lab7
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
			app.CreatePerOwinContext(ApplicationDbContext.Create);
			app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
			app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);
			app.CreatePerOwinContext<ApplicationRoleManager>(ApplicationRoleManager.Create);
			ConfigureAuth(app);
        }
    }
}
