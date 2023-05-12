namespace UniformancePhdConnectSystem.WebApi.Controllers
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.Owin;
    using System.Net.Http;
    using System.Web.Http;
    using UniformancePhdConnectSystem.Data;
    using UniformancePhdConnectSystem.WebApi.Models;

    public class BaseApiController : ApiController
    {
        private ModelFactory modelFactory;
        private ApplicationUserManager userManager = null;

        public BaseApiController()
        {

        }

        protected ApplicationUserManager UserManager => 
            userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();

        protected ModelFactory ModelFactory
        {
            get
            {
                if (modelFactory == null)
                {
                    modelFactory = new ModelFactory(this.Request, this.UserManager);
                }

                return modelFactory;
            }
        }

        protected IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }
    }
}