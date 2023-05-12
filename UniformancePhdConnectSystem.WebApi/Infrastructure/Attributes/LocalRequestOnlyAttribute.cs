namespace UniformancePhdConnectSystem.WebApi.Infrastructure.Attributes
{
    using Microsoft.Owin;
    using Serilog;
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Web;
    using System.Web.Http;
    using System.Web.Http.Controllers;

    public class LocalRequestOnlyAttribute : AuthorizeAttribute
    {
        private readonly ILogger logger = Log.ForContext<LocalRequestOnlyAttribute>();

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            var ip = GetClientIpAddress(actionContext.Request);
            this.logger.Information($"Request from {ip}");

            return actionContext.RequestContext.IsLocal;
        }

        private string GetClientIpAddress(HttpRequestMessage request)
        {
            if (request.Properties.ContainsKey("MS_HttpContext"))
            {
                return IPAddress.Parse(((HttpContextBase)request.Properties["MS_HttpContext"]).Request.UserHostAddress).ToString();
            }
            if (request.Properties.ContainsKey("MS_OwinContext"))
            {
                return IPAddress.Parse(((OwinContext)request.Properties["MS_OwinContext"]).Request.RemoteIpAddress).ToString();
            }
            return String.Empty;
        }
    }
}