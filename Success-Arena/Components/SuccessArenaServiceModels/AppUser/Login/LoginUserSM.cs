using SuccessArenaServiceModels.Enums;
using SuccessArenaServiceModels.Foundation.Base;
using SuccessArenaServiceModels.Foundation.Base.CommonResponseRoot;
using SuccessArenaServiceModels.Foundation.Base.Enums;

namespace SuccessArenaServiceModels.AppUser.Login
{
    public class LoginUserSM : SuccessArenaServiceModelBase<int>
    {
        public LoginUserSM()
        {
        }
        public RoleTypeSM RoleType { get; set; }
        public string LoginId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        [IgnorePropertyOnWrite(AutoMapConversionType.Dm2SmOnly)]
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string? ProfilePictureUrl { get; set; }

        public bool IsPhoneNumberConfirmed { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public LoginStatusSM LoginStatus { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }
}
