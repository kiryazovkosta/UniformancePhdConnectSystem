using System.Web.Http;
using WebActivatorEx;
using UniformancePhdConnectSystem.WebApi;
using Swashbuckle.Application;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace UniformancePhdConnectSystem.WebApi
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                    {
                        c.SingleApiVersion("v1", "UniformancePhdConnectSystem.WebApi");
                    })
                .EnableSwaggerUi(c =>
                    {
                    });
        }
    }
}
