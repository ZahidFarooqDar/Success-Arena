using SuccessArenaDomainModels.Foundation.Base;

namespace SuccessArenaDomainModels.v1
{
    public class ExamSubjectDM : SuccessArenaDomainModelBase<int>
    {
        public int ExamId { get; set; }

        public int SubjectId { get; set; }

        public virtual ExamDM Exam { get; set; }

        public virtual SubjectDM Subject { get; set; }
    }
}
