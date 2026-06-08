using SuccessArenaServiceModels.Foundation.Base;

namespace SuccessArenaServiceModels.v1
{
    public class InvoiceSM : SuccessArenaServiceModelBase<int>
    {
        public int ClientUserId { get; set; }

        public int UserTestPackageId { get; set; }

        public string InvoiceNumber { get; set; }

        public decimal TotalAmount { get; set; }

        public string? PaymentReference { get; set; }

        public string? PaymentGateway { get; set; }

        public string? TransactionId { get; set; }
    }
}