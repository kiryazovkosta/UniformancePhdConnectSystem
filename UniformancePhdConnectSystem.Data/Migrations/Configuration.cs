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
                var manager = new UserManager<ApplicationUser>(
                    new UserStore<ApplicationUser>(
                        new ApplicationDbContext()));

                var user = new ApplicationUser
                {
                    UserName = "root",
                    Email = "kosta.kiryazov@ensicontrol.eu",
                    EmailConfirmed = true,
                    FirstName = "Kosta",
                    LastName = "Kiryazov" 
                };

                manager.Create(user, "M!P@ssW0rd");
            }
        }
    }
}