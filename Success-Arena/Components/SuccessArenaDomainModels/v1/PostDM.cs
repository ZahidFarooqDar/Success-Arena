using SuccessArenaDomainModels.Foundation.Base;

namespace SuccessArenaDomainModels.v1
{
    public class PostDM : SuccessArenaDomainModelBase<int>
    {
        public string Name { get; set; }

        public string Code { get; set; }

        public string? Description { get; set; }

        public bool IsActive { get; set; }

        public virtual ICollection<ExamPostDM> ExamPosts { get; set; }
            = new List<ExamPostDM>();

        public virtual ICollection<PostSubjectDM> PostSubjects { get; set; }
            = new List<PostSubjectDM>();

        public virtual ICollection<QuestionDM> Questions { get; set; }
            = new List<QuestionDM>();

        public virtual ICollection<TestPackageDM> TestPackages { get; set; }
            = new List<TestPackageDM>();
    }
}
