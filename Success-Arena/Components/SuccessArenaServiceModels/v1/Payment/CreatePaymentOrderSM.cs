namespace SuccessArenaServiceModels.v1.Payment
{
    public class CreatePaymentOrderSM
    {
        public int PackageId { get; set; }
    }
    public class RazorPayOrderSM
    {
        public string OrderId { get; set; }

        public decimal Amount { get; set; }

        public string Currency { get; set; }

        public int UserPackageId { get; set; }
    }

    public class VerifyPaymentSM
    {
        public int UserPackageId { get; set; }

        public string RazorPayOrderId { get; set; }

        public string RazorPayPaymentId { get; set; }

        public string RazorPaySignature { get; set; }
    }
}
