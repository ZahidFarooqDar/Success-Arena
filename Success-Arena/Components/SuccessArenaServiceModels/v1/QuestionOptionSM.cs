using SuccessArenaServiceModels.Foundation.Base;

namespace SuccessArenaServiceModels.v1
{
    public class QuestionOptionSM : SuccessArenaServiceModelBase<int>
    {
        public int QuestionId { get; set; }

        public string OptionLabel { get; set; }

        public string? OptionText { get; set; }

        public string? OptionImageUrl { get; set; }

        public bool IsCorrect { get; set; }

        public int DisplayOrder { get; set; }
    }
}