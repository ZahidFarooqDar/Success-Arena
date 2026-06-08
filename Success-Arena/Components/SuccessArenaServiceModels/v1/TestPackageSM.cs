using SuccessArenaServiceModels.Foundation.Base;

namespace SuccessArenaServiceModels.v1
{
    public class TestPackageSM : SuccessArenaServiceModelBase<int>
    {
        public string Name { get; set; }

        public string? Description { get; set; }

        public int PostId { get; set; }

        public int? SubjectId { get; set; }

        public decimal Price { get; set; }

        public int TotalQuestions { get; set; }

        public int DurationInMinutes { get; set; }

        public int AllowedAttempts { get; set; }

        public bool IsFree { get; set; }

        public bool IsActive { get; set; }
    }
}