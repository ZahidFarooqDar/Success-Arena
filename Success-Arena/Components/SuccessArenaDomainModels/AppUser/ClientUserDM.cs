using SuccessArenaDomainModels.AppUser.Login;
using SuccessArenaDomainModels.Enums;
using SuccessArenaDomainModels.v1;
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

        public virtual ICollection<UserTestPackageDM> UserTestPackages { get; set; }
    = new List<UserTestPackageDM>();

        public virtual ICollection<TestAttemptDM> TestAttempts { get; set; }
            = new List<TestAttemptDM>();

        public virtual ICollection<InvoiceDM> Invoices { get; set; }
            = new List<InvoiceDM>();

    }
}
