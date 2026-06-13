using SuccessArenaServiceModels.Enums;
using SuccessArenaServiceModels.Foundation.Base;

namespace SuccessArenaServiceModels.AppUser
{
    public class ClientUserSM : SuccessArenaServiceModelBase<int>
    {
        public string? Name { get; set; }
        public string Email { get; set; }

        public string? PhoneNumber { get; set; }
        public string? Password { get; set; }

        public string? ImageUrl { get; set; }

        public string? PaymentId { get; set; }

        public string? FirebaseId { get; set; }

        public RoleTypeSM RoleType { get; set; }

        public bool IsEmailConfirmed { get; set; }
        public bool IsPhoneNumberConfirmed { get; set; }
        public LoginStatusSM LoginStatus { get; set; }

    }
}
