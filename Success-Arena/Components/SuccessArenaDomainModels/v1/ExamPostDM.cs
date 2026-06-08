using SuccessArenaDomainModels.Foundation.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace SuccessArenaDomainModels.v1
{
    public class ExamPostDM : SuccessArenaDomainModelBase<int>
    {
        [ForeignKey(nameof(Post))]
        public int PostId { get; set; }
        public virtual PostDM Post { get; set; }

        [ForeignKey(nameof(Exam))]
        public int ExamId { get; set; }

        public virtual ExamDM Exam { get; set; }
        
    }
}
