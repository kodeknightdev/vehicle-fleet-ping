using System.Collections.Generic;

namespace FleetPing.Api.Auth
{
    public class BasicAuthOptions
    {
        public const string Section = "Auth";

        public List<AuthUser> Users { get; set; } = new List<AuthUser>();
    }
}
