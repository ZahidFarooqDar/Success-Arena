using SuccessArenaServiceModels.AppUser.Login;
using SuccessArenaServiceModels.Enums;

namespace SuccessArenaServiceModels.AppUser
{
    public class ClientUserSM : LoginUserSM
    {
        public GenderSM Gender { get; set; }
        public string PersonalEmailId { get; set; }
        public string? PaymentId { get; set; }
        public int? ClientCompanyDetailId { get; set; }

        public string? FirebaseId { get; set; }


    }
}
