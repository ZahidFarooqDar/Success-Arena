using SuccessArenaDomainModels.AppUser;
using SuccessArenaDomainModels.Foundation.Base;

namespace SuccessArenaDomainModels.v1
{
    public class TestAttemptDM : SuccessArenaDomainModelBase<int>
    {
        public int ClientUserId { get; set; }

        public int TestPackageId { get; set; }

        public int ExamId { get; set; }

        public int? SubjectId { get; set; }

        public int TotalQuestions { get; set; }

        public int CorrectAnswers { get; set; }

        public int WrongAnswers { get; set; }

        public decimal Percentage { get; set; }

        public int TimeTakenInMinutes { get; set; }

        public DateTime StartedOn { get; set; }

        public DateTime SubmittedOn { get; set; }

        public virtual ClientUserDM ClientUser { get; set; }

        public virtual ICollection<TestAttemptAnswerDM> Answers { get; set; }
    }
}
