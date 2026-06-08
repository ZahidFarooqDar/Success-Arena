using SuccessArenaDomainModels.Foundation.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace SuccessArenaDomainModels.v1
{
    public class QuestionDM : SuccessArenaDomainModelBase<int>
    {
        [ForeignKey(nameof(Post))]
        public int PostId { get; set; }

        public virtual PostDM Post { get; set; }

        [ForeignKey(nameof(Subject))]
        public int SubjectId { get; set; }

        public virtual SubjectDM Subject { get; set; }

        public string? QuestionText { get; set; }

        public string? QuestionImageUrl { get; set; }

        public string? Explanation { get; set; }

        public string? ExplanationImageUrl { get; set; }

        public bool IsActive { get; set; }

        public virtual ICollection<QuestionOptionDM> QuestionOptions { get; set; }
            = new List<QuestionOptionDM>();

        public virtual ICollection<TestAttemptAnswerDM> TestAttemptAnswers { get; set; }
            = new List<TestAttemptAnswerDM>();
    }
}
