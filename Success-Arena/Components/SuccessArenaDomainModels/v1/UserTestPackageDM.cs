using SuccessArenaDomainModels.AppUser;
using SuccessArenaDomainModels.Enums;
using SuccessArenaDomainModels.Foundation.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace SuccessArenaDomainModels.v1
{
    public class UserTestPackageDM : SuccessArenaDomainModelBase<int>
    {
        [ForeignKey(nameof(ClientUser))]
        public int ClientUserId { get; set; }

        public virtual ClientUserDM ClientUser { get; set; }

        [ForeignKey(nameof(TestPackage))]
        public int TestPackageId { get; set; }

        public virtual TestPackageDM TestPackage { get; set; }

        public int AllowedAttempts { get; set; }

        public PaymentStatusDM PaymentStatus { get; set; }
        public int AttemptsUsed { get; set; }

        public string? RecieptId { get; set; }

        public DateTime PurchasedOn { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public bool IsActive { get; set; }

        public virtual ICollection<TestAttemptDM> TestAttempts { get; set; }
            = new List<TestAttemptDM>();

        public virtual ICollection<InvoiceDM> Invoices { get; set; }
            = new List<InvoiceDM>();
    }
}
