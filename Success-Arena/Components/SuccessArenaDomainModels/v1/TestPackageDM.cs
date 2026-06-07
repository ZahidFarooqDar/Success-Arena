using SuccessArenaDomainModels.Foundation.Base;

namespace SuccessArenaDomainModels.v1
{
    public class TestPackageDM : SuccessArenaDomainModelBase<int>
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public int ExamId { get; set; }

        /// NULL = Full Exam Test
        public int? SubjectId { get; set; }

        public decimal Price { get; set; }

        public int TotalQuestions { get; set; }

        public int DurationInMinutes { get; set; }

        /// Number of attempts allowed
        public int AllowedAttempts { get; set; }

        public bool IsFree { get; set; }

        public bool IsActive { get; set; }

        public virtual ExamDM Exam { get; set; }

        public virtual SubjectDM Subject { get; set; }
    }
}
