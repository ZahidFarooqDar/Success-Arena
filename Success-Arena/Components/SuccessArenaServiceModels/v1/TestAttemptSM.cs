using SuccessArenaServiceModels.Enums;
using SuccessArenaServiceModels.Foundation.Base;

namespace SuccessArenaServiceModels.v1
{
    public class TestAttemptSM : SuccessArenaServiceModelBase<int>
    {
        public int ClientUserId { get; set; }

        public int UserTestPackageId { get; set; }

        public int ExamId { get; set; }

        public int? SubjectId { get; set; }
        public TestAttemptStatusSM Status { get; set; }
        public int TotalQuestions { get; set; }

        public int CorrectAnswers { get; set; }

        public int WrongAnswers { get; set; }

        public decimal Percentage { get; set; }

        public int TimeTakenInMinutes { get; set; }

        public DateTime StartedOn { get; set; }

        public DateTime? SubmittedOn { get; set; }
    }
}