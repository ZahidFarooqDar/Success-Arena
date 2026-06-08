using SuccessArenaServiceModels.Foundation.Base;

namespace SuccessArenaServiceModels.v1
{
    public class TestAttemptAnswerSM : SuccessArenaServiceModelBase<int>
    {
        public int TestAttemptId { get; set; }

        public int QuestionId { get; set; }

        public string? SelectedOption { get; set; }

        public string CorrectOption { get; set; }

        public bool IsCorrect { get; set; }

        public DateTime AnsweredOn { get; set; }
    }
}