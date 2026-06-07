using SuccessArenaDomainModels.Foundation.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace SuccessArenaDomainModels.v1
{
    public class ExamDM : SuccessArenaDomainModelBase<int>
    {

        public string Name { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }
        public virtual ICollection<ExamBoardExamDM> ExamBoardExams { get; set; }

        public virtual ICollection<ExamSubjectDM> ExamSubjects { get; set; }
    }
}
