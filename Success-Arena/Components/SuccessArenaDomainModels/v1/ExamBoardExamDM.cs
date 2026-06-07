using SuccessArenaDomainModels.Foundation.Base;

namespace SuccessArenaDomainModels.v1
{
    public class ExamBoardExamDM : SuccessArenaDomainModelBase<int>
    {
        public int ExamBoardId { get; set; }

        public int ExamId { get; set; }

        public virtual ExamBoardDM ExamBoard { get; set; }

        public virtual ExamDM Exam { get; set; }
    }
}
