using SuccessArenaServiceModels.Foundation.Base;

namespace SuccessArenaServiceModels.v1
{
    public class UserTestPackageSM : SuccessArenaServiceModelBase<int>
    {
        public int ClientUserId { get; set; }

        public int TestPackageId { get; set; }

        public int AllowedAttempts { get; set; }

        public string? ReceiptId { get; set; }

        public int AttemptsUsed { get; set; }

        public DateTime PurchasedOn { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public bool IsActive { get; set; }
    }
}