using Microsoft.AspNetCore.Authentication;

namespace SuccessArenaFoundation.Security
{
    public class SuccessArenaAuthenticationSchemeOptions : AuthenticationSchemeOptions
    {
        public string JwtTokenSigningKey { get; set; }
    }
}
