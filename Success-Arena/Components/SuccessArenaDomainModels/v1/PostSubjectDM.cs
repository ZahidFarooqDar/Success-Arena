using SuccessArenaDomainModels.Foundation.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace SuccessArenaDomainModels.v1
{
    public class PostSubjectDM : SuccessArenaDomainModelBase<int>
    {
        [ForeignKey(nameof(Post))]
        public int PostId { get; set; }

        public virtual PostDM Post { get; set; }

        [ForeignKey(nameof(Subject))]
        public int SubjectId { get; set; }

        public virtual SubjectDM Subject { get; set; }
    }
}
