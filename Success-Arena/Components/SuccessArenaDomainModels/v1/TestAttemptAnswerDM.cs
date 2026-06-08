using SuccessArenaDomainModels.Foundation.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace SuccessArenaDomainModels.v1
{
    public class TestAttemptAnswerDM : SuccessArenaDomainModelBase<int>
    {
        [ForeignKey(nameof(TestAttempt))]
        public int TestAttemptId { get; set; }

        public virtual TestAttemptDM TestAttempt { get; set; }

        [ForeignKey(nameof(Question))]
        public int QuestionId { get; set; }

        public virtual QuestionDM Question { get; set; }

        public string? SelectedOption { get; set; }

        public string CorrectOption { get; set; }

        public bool IsCorrect { get; set; }

        public DateTime AnsweredOn { get; set; }
    }
}
