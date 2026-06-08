using SuccessArenaDomainModels.Foundation.Base;

namespace SuccessArenaDomainModels.v1
{
    public class ExamDM : SuccessArenaDomainModelBase<int>
    {
        public string Name { get; set; }

        public string Code { get; set; } // JKSSB, SSC, JKBANK

        public string Description { get; set; }

        public string IconUrl { get; set; }

        public bool IsActive { get; set; }

        public virtual ICollection<ExamPostDM> ExamPosts { get; set; }
        = new List<ExamPostDM>();
    }
}
