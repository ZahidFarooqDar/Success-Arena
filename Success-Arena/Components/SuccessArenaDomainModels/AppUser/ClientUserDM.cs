using SuccessArenaDomainModels.AppUser.Login;
using SuccessArenaDomainModels.Enums;
using SuccessArenaDomainModels.Foundation.Base;
using SuccessArenaDomainModels.v1;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace SuccessArenaDomainModels.AppUser
{
    public class ClientUserDM : SuccessArenaDomainModelBase<int>
    {
        
        public string? Name { get; set; }
        [Required]
        public string Email { get; set; }

        public string? PhoneNumber { get; set; }
        public string? Password { get; set; }

        public string? ImageUrl { get; set; }

        public string? PaymentId { get; set; }

        public string? FirebaseId { get; set; }

        [NotNull]
        [Required]
        public RoleTypeDM RoleType { get; set; }

        [DefaultValue(false)]
        public bool IsEmailConfirmed { get; set; }
        [DefaultValue(false)]
        public bool IsPhoneNumberConfirmed { get; set; }
        public LoginStatusDM LoginStatus { get; set; }

        public virtual ICollection<UserTestPackageDM> UserTestPackages { get; set; }
    = new List<UserTestPackageDM>();

        public virtual ICollection<TestAttemptDM> TestAttempts { get; set; }
            = new List<TestAttemptDM>();

        public virtual ICollection<InvoiceDM> Invoices { get; set; }
            = new List<InvoiceDM>();

    }
}
