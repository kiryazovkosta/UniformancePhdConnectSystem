namespace UniformancePhdConnectSystem.WebApi.Controllers
{
    using Serilog;
    using System.Web.Http;

    [RoutePrefix("api/default")]
    public class DefaultController : ApiController
    {
        private readonly ILogger logger = Log.ForContext<DefaultController>();

        [HttpGet]
        [Route("get")]
        public IHttpActionResult Get()
        {
            logger.Information("GET method");
            return Ok("Ок");
        }
    }
}
