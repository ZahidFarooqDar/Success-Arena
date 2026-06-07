using SuccessArenaDomainModels.AppUser;
using SuccessArenaDomainModels.Foundation.Base;

namespace SuccessArenaDomainModels.v1
{
    public class UserTestPackageDM : SuccessArenaDomainModelBase<int>
    {
        public int ClientUserId { get; set; }

        public int TestPackageId { get; set; }

        public int InvoiceId { get; set; }

        public int AllowedAttempts { get; set; }

        public int AttemptsUsed { get; set; }

        public DateTime ActivatedOn { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public bool IsActive { get; set; }

        public virtual ClientUserDM ClientUser { get; set; }

        public virtual TestPackageDM TestPackage { get; set; }

        public virtual InvoiceDM Invoice { get; set; }
    }
}
