using SuccessArenaDomainModels.Foundation.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace SuccessArenaDomainModels.v1
{
    public class TestPackageDM : SuccessArenaDomainModelBase<int>
    {
        public string Name { get; set; }

        public string? Description { get; set; }

        [ForeignKey(nameof(Post))]
        public int PostId { get; set; }

        public virtual PostDM Post { get; set; }

        [ForeignKey(nameof(Subject))]
        public int? SubjectId { get; set; }

        public virtual SubjectDM? Subject { get; set; }

        public decimal Price { get; set; }

        public int TotalQuestions { get; set; }

        public int DurationInMinutes { get; set; }

        public int AllowedAttempts { get; set; }

        public bool IsFree { get; set; }

        public bool IsActive { get; set; }

        public virtual ICollection<UserTestPackageDM> UserTestPackages { get; set; }
            = new List<UserTestPackageDM>();
    }
}
