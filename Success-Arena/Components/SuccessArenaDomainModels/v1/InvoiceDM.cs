using SuccessArenaDomainModels.AppUser;
using SuccessArenaDomainModels.Foundation.Base;

namespace SuccessArenaDomainModels.v1
{
    public class InvoiceDM : SuccessArenaDomainModelBase<int>
    {
        public int ClientUserId { get; set; }

        public string InvoiceNumber { get; set; }

        public decimal TotalAmount { get; set; }

        public string PaymentReference { get; set; }

        public string PaymentGateway { get; set; }

        public string TransactionId { get; set; }

        public virtual ClientUserDM ClientUser { get; set; }

    }
}
