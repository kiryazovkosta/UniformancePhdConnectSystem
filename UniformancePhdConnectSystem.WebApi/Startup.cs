namespace UniformancePhdConnectSystem.WebApi
{
    using Microsoft.Owin;
    using Microsoft.Owin.BuilderProperties;
    using Microsoft.Owin.Security;
    using Microsoft.Owin.Security.DataHandler.Encoder;
    using Microsoft.Owin.Security.Jwt;
    using Microsoft.Owin.Security.OAuth;
    using Newtonsoft.Json.Serialization;
    using Owin;
    using Serilog;
    using System;
    using System.Configuration;
    using System.Linq;
    using System.Net.Http.Formatting;
    using System.Web.Http;
    using UniformancePhdConnectSystem.Data;
    using UniformancePhdConnectSystem.WebApi.Data;
    using UniformancePhdConnectSystem.WebApi.Providers;

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.AppSettings()
                .CreateLogger();

            InitializeUniformancePhdConnection();
            ReleaseUniformancePhdConnection(app);
            RunOwin(app);
        }

        private void RunOwin(IAppBuilder app)
        {
            var httpConfig = new HttpConfiguration();
            ConfigureOAuthTokenGeneration(app);
            ConfigureOAuthTokenConsumption(app);
            ConfigureWebApi(httpConfig);
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            app.UseWebApi(httpConfig);
        }

        private static void ReleaseUniformancePhdConnection(IAppBuilder app)
        {
            var properties = new AppProperties(app.Properties);
            var token = properties.OnAppDisposing;
            if (token != System.Threading.CancellationToken.None)
            {
                token.Register(() => PerformShutdown("OWIN OnAppDisposing"));
            }

            AppDomain.CurrentDomain.DomainUnload += (s, e) => PerformShutdown("AppDomain.DomainUnload (fallback)");

            AppDomain.CurrentDomain.ProcessExit += (s, e) => PerformShutdown("AppDomain.ProcessExit (safety net)");
        }

        private static void InitializeUniformancePhdConnection()
        {
            try
            {
                UniformancePhdProvider.Instance.Initialize("10.94.0.241", 3100);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to initialize PHD Provider during OWIN startup.");
            }
        }

        private static void PerformShutdown(string reason)
        {
            Log.Information("PHD Shutdown triggered by {Reason}.", reason);
            UniformancePhdProvider.Instance.Shutdown();
            Log.CloseAndFlush();
        }

        private void ConfigureOAuthTokenGeneration(IAppBuilder app)
        {
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationRoleManager>(ApplicationRoleManager.Create);

            OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = false,
                TokenEndpointPath = new PathString("/oauth/token"),
                AccessTokenExpireTimeSpan = 
                    TimeSpan.FromMinutes(double.Parse(ConfigurationManager.AppSettings["as:Timeout"])),
                Provider = new CustomOAuthProvider(),
                AccessTokenFormat = new CustomJwtFormat(ConfigurationManager.AppSettings["as:BaseUrl"])
            };

            // OAuth 2.0 Bearer Access Token Generation
            app.UseOAuthAuthorizationServer(OAuthServerOptions);
        }

        private void ConfigureOAuthTokenConsumption(IAppBuilder app)
        {

            var issuer = ConfigurationManager.AppSettings["as:BaseUrl"];
            string audienceId = ConfigurationManager.AppSettings["as:AudienceId"];
            byte[] audienceSecret = TextEncodings.Base64Url.Decode(ConfigurationManager.AppSettings["as:AudienceSecret"]);

            // Api controllers with an [Authorize] attribute will be validated with JWT
            app.UseJwtBearerAuthentication(
                new JwtBearerAuthenticationOptions
                {
                    AuthenticationMode = AuthenticationMode.Active,
                    AllowedAudiences = new[] { audienceId },
                    IssuerSecurityKeyProviders = new IIssuerSecurityKeyProvider[]
                    {
                        new SymmetricKeyIssuerSecurityKeyProvider(issuer, audienceSecret)
                    }
                });
        }

        private void ConfigureWebApi(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            var appXmlType = config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "application/xml");
            config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(appXmlType);
        }
    }
}