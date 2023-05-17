namespace UniformancePhdConnectSystem.Data.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using UniformancePhdConnectSystem.WebApi.Data;
    using UniformancePhdConnectSystem.WebApi.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            if (!context.Set<ApplicationUser>().Any())
            {
                var userManager = new UserManager<ApplicationUser>(
                    new UserStore<ApplicationUser>(
                        new ApplicationDbContext()));

                var roleManager = new RoleManager<IdentityRole>
                    (new RoleStore<IdentityRole>(
                        new ApplicationDbContext()));

                var rootUser = new ApplicationUser
                {
                    UserName = "root",
                    Email = "kosta.kiryazov@ensicontrol.eu",
                    EmailConfirmed = true,
                    FirstName = "Kosta",
                    LastName = "Kiryazov" 
                };

                var fetchUser = new ApplicationUser
                {
                    UserName = "fetcher",
                    Email = "phd-fetch@ensicontrol.eu",
                    EmailConfirmed = true,
                    FirstName = "System",
                    LastName = "Fetcher"
                };

                userManager.Create(rootUser, "M!P@ssW0rd1s");
                userManager.Create(fetchUser, "iW1LFet4UphdD@t@z3_#");

                if (!roleManager.Roles.Any())
                {
                    roleManager.Create(new IdentityRole { Name = "Admin" });
                    roleManager.Create(new IdentityRole { Name = "PhdUser" });
                }

                var adminUser = userManager.FindByName("root");
                userManager.AddToRole(adminUser.Id, "Admin");

                var phdUser = userManager.FindByName("fetcher");
                userManager.AddToRole(phdUser.Id, "PhdUser");
            }
        }
    }
}