using SuccessArenaDomainModels.Foundation.Base;

namespace SuccessArenaDomainModels.v1
{
    public class QuestionDM : SuccessArenaDomainModelBase<int>
    {
        public int ExamId { get; set; }

        public int SubjectId { get; set; }

        public string? QuestionText { get; set; }

        public string? QuestionImageUrl { get; set; }

        public string? Explanation { get; set; }

        public string? ExplanationImageUrl { get; set; }


        public bool IsActive { get; set; }

        public virtual ExamDM Exam { get; set; }

        public virtual SubjectDM Subject { get; set; }

        public virtual ICollection<QuestionOptionDM> Options { get; set; }
    }
}
