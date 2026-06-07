using SuccessArenaDomainModels.AppUser.Login;
using SuccessArenaDomainModels.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace SuccessArenaDomainModels.AppUser
{
    public class ClientUserDM : LoginUserDM
    {
        public ClientUserDM()
        {
        }
        public GenderDM? Gender { get; set; }
        public string? PaymentId { get; set; }

        public string? FirebaseId { get; set; }

    }
}
