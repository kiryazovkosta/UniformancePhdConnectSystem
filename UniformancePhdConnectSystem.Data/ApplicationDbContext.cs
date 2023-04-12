namespace UniformancePhdConnectSystem.WebApi.Data
{
    using Microsoft.AspNet.Identity.EntityFramework;
    using UniformancePhdConnectSystem.WebApi.Models;

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            Configuration.ProxyCreationEnabled = false;
            Configuration.LazyLoadingEnabled = false;
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}