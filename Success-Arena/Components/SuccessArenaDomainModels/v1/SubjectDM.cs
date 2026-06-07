using SuccessArenaDomainModels.Foundation.Base;

namespace SuccessArenaDomainModels.v1
{
    public class SubjectDM : SuccessArenaDomainModelBase<int>
    {       
        public string Name { get; set; }

        public string Code { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<ExamSubjectDM> ExamSubjects { get; set; }
    }
}
