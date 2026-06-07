using SuccessArenaDomainModels.Foundation.Base;

namespace SuccessArenaDomainModels.v1
{
    public class TestAttemptAnswerDM : SuccessArenaDomainModelBase<int>
    {
        public int TestAttemptId { get; set; }

        public int QuestionId { get; set; }

        public string SelectedOption { get; set; }

        public string CorrectOption { get; set; }

        public bool IsCorrect { get; set; }

        public DateTime AnsweredOn { get; set; }

        public virtual TestAttemptDM TestAttempt { get; set; }

        public virtual QuestionDM Question { get; set; }
    }
}
