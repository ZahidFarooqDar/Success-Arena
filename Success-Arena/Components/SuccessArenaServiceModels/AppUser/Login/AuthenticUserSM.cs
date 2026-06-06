using Microsoft.AspNetCore.Identity;
using SuccessArenaServiceModels.Enums;

namespace SuccessArenaServiceModels.AppUser.Login
{
    public class AuthenticUserSM : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public RoleTypeSM Role { get; set; }
    }
}
