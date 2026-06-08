using SuccessArenaDomainModels.AppUser;
using SuccessArenaDomainModels.Foundation.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace SuccessArenaDomainModels.v1
{
    public class InvoiceDM : SuccessArenaDomainModelBase<int>
    {
        [ForeignKey(nameof(ClientUser))]
        public int ClientUserId { get; set; }

        public virtual ClientUserDM ClientUser { get; set; }

        [ForeignKey(nameof(UserTestPackage))]
        public int UserTestPackageId { get; set; }

        public virtual UserTestPackageDM UserTestPackage { get; set; }

        public string InvoiceNumber { get; set; }

        public decimal TotalAmount { get; set; }

        public string? PaymentReference { get; set; }

        public string? PaymentGateway { get; set; }

        public string? TransactionId { get; set; }
    }
}
