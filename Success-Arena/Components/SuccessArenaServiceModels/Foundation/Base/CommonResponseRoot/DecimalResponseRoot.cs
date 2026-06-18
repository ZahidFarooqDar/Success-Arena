namespace SuccessArenaServiceModels.Foundation.Base.CommonResponseRoot
{
    public class DecimalResponseRoot
    {
        public decimal DecimalResponse { get; set; }
        public string? Message { get; set; }
        public DecimalResponseRoot(decimal decimalResponse, string message = "")
        {
            DecimalResponse = decimalResponse;
            Message = message;
        }
    }
}
