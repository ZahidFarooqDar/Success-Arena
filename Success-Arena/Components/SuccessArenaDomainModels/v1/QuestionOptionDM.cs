using SuccessArenaDomainModels.Foundation.Base;

namespace SuccessArenaDomainModels.v1
{
    public class QuestionOptionDM : SuccessArenaDomainModelBase<int>
    {
        public int QuestionId { get; set; }

        public string OptionLabel { get; set; }

        public string? OptionText { get; set; }

        public string? OptionImageUrl { get; set; }

        public bool IsCorrect { get; set; }

        public int DisplayOrder { get; set; }

        public virtual QuestionDM Question { get; set; }
    }
}
