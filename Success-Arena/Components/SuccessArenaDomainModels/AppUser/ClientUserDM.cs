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


    }
}
