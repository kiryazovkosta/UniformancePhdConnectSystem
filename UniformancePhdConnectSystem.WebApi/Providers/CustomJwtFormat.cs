namespace UniformancePhdConnectSystem.WebApi.Providers
{
    using Microsoft.Owin.Security;
    using Microsoft.Owin.Security.DataHandler.Encoder;
    using System;
    using System.Configuration;
    using System.IdentityModel.Tokens.Jwt;
    using Thinktecture.IdentityModel.Tokens;
    using Microsoft.IdentityModel.Tokens;

    public class CustomJwtFormat : ISecureDataFormat<AuthenticationTicket>
    {
        private readonly string issuer = string.Empty;

        public CustomJwtFormat(string issuer)
        {
            this.issuer = issuer;
        }

        public string Protect(AuthenticationTicket data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            string audienceId = ConfigurationManager.AppSettings["as:AudienceId"];
            string symmetricKeyAsBase64 = ConfigurationManager.AppSettings["as:AudienceSecret"];
            var keyByteArray = TextEncodings.Base64Url.Decode(symmetricKeyAsBase64);
            //var signingKey = new HmacSigningCredentials(keyByteArray);
            DateTimeOffset? issued = data.Properties.IssuedUtc ?? 
                         DateTime.UtcNow;
            DateTimeOffset? expires = data.Properties.ExpiresUtc ??
                                      DateTime.UtcNow.AddMinutes(
                                          double.Parse(ConfigurationManager.AppSettings["as:Timeout"]));
            var securityKey = new SymmetricSecurityKey(keyByteArray);
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken(
                issuer, 
                audienceId, 
                data.Identity.Claims, 
                issued.Value.UtcDateTime, 
                expires.Value.UtcDateTime,
                signingCredentials);
            var handler = new JwtSecurityTokenHandler();

            var jwt = handler.WriteToken(token);
            return jwt;
        }

        public AuthenticationTicket Unprotect(string protectedText)
        {
            throw new NotImplementedException();
        }
    }
}