namespace UniformancePhdConnectSystem.WebApi.Models
{
    using Microsoft.AspNet.Identity.EntityFramework;
    using System.Net.Http;
    using System.Web.Http.Routing;
    using UniformancePhdConnectSystem.Data;
    using UniformancePhdConnectSystem.Models.Identity;

    public class ModelFactory
    {
        private UrlHelper urlHelper;
        private ApplicationUserManager userManager;

        public ModelFactory(HttpRequestMessage request, ApplicationUserManager userManager)
        {
            urlHelper = new UrlHelper(request);
            this.userManager = userManager;
        }

        public UserViewModel Create(ApplicationUser appUser)
        {
            return new UserViewModel
            {
                Url = urlHelper.Link("GetUserById", new { id = appUser.Id }),
                Id = appUser.Id,
                UserName = appUser.UserName,
                FullName = string.Format("{0} {1}", appUser.FirstName, appUser.LastName),
                Email = appUser.Email,
                EmailConfirmed = appUser.EmailConfirmed,
                Roles = userManager.GetRolesAsync(appUser.Id).Result,
                Claims = userManager.GetClaimsAsync(appUser.Id).Result
            };
        }

        public RoleViewModel Create(IdentityRole appRole)
        {

            return new RoleViewModel
            {
                Url = urlHelper.Link("GetRoleById", new { id = appRole.Id }),
                Id = appRole.Id,
                Name = appRole.Name
            };
        }
    }
}