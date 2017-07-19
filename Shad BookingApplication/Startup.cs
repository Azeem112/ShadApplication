using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using Shad_BookingApplication.Models;
using System.Web.Optimization;

[assembly: OwinStartupAttribute(typeof(Shad_BookingApplication.Startup))]
namespace Shad_BookingApplication
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            BundleTable.EnableOptimizations = true;
            // createRolesandUsers();
            CreateRolesandUsers();

        }
        private void createRolesandUsers()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));


            // In Startup iam creating first Admin Role and creating a default Admin User    
            if (!roleManager.RoleExists("Super_Admin"))
            {

                // first we create Admin rool  
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Super_Admin";
                roleManager.Create(role);

                //Here we create a Admin super user who will maintain the website                  

                var user = new ApplicationUser();
                user.UserName = "Ahmad1";
                user.Email = "Ahmad@gmail.com";

                string userPWD = "Ahmad@1234";

                var chkUser = UserManager.Create(user, userPWD);

                //Add default User to Role Admin  
                if (chkUser.Succeeded)
                {
                    var result1 = UserManager.AddToRole(user.Id, "Super_Admin");

                }
            }

            // creating Creating Company_Admin role    
            if (!roleManager.RoleExists("Company_Admin"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Company_Admin";
                roleManager.Create(role);

            }

            // creating Creating Employee role    
            if (!roleManager.RoleExists("Employee"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Employee";
                roleManager.Create(role);
            }
        }

        private void CreateRolesandUsers()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));


            // In Startup iam creating first Admin Role and creating a default Admin User    
            if (!roleManager.RoleExists("Super_Admin"))
            {

                // first we create Admin rool   
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Super_Admin";
                roleManager.Create(role);

                //Here we create a Admin super user who will maintain the website                  

                var user = new ApplicationUser();
                user.UserName = "Bilal";
                user.Email = "Bilal@gmail.com";
                user.PhoneNumber = "";
            
                string userPWD = "Bilal@1234";

                var chkUser = UserManager.Create(user, userPWD);

                //Add default User to Role Admin   
                if (chkUser.Succeeded)
                {
                    var result1 = UserManager.AddToRole(user.Id, "Super_Admin");
                }
            }

            // creating Creating Company_Admin role    
            if (!roleManager.RoleExists("Company_Admin"))
            {
                var role1 = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role1.Name = "Company_Admin";
                roleManager.Create(role1);
           }

            // creating Creating Employee role    
            if (!roleManager.RoleExists("Employee"))
            {
                var role2 = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role2.Name = "Employee";
                roleManager.Create(role2);

           }
        }
    }
}
   
