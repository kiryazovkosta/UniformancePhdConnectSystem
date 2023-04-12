using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace UniformancePhdConnectSystem.WebApi.Controllers
{
    [RoutePrefix("api/default")]
    public class DefaultController : ApiController
    {
        [HttpGet]
        [Route("get")]
        public IHttpActionResult Get()
        {
            return Ok("Ок");
        }
    }
}
