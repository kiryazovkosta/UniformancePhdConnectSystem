namespace UniformancePhdConnectSystem.Models.Identity
{
    using System.Collections.Generic;
    using System.Security.Claims;

    public class UserViewModel
    {
        public string Url { get; set; }
        public string Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public IList<string> Roles { get; set; }
        public IList<Claim> Claims { get; set; }
    }
}