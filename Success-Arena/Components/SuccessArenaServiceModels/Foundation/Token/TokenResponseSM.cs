using SuccessArenaServiceModels.AppUser.Login;

namespace SuccessArenaServiceModels.Foundation.Token
{
    public class TokenResponseSM : TokenResponseRoot
    {
        public LoginUserSM LoginUserDetails { get; set; }
        public int ClientCompanyId { get; set; }
    }
}
