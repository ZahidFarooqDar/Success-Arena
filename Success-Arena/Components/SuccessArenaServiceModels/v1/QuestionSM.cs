using SuccessArenaServiceModels.Foundation.Base;

namespace SuccessArenaServiceModels.v1
{
    public class QuestionSM : SuccessArenaServiceModelBase<int>
    {
        public int PostId { get; set; }

        public int SubjectId { get; set; }

        public string? QuestionText { get; set; }

        public string? QuestionImageUrl { get; set; }

        public string? Explanation { get; set; }

        public string? ExplanationImageUrl { get; set; }

        public bool IsActive { get; set; }
    }
}