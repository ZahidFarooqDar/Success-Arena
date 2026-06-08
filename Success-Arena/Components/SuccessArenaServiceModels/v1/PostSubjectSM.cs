using SuccessArenaServiceModels.Foundation.Base;

namespace SuccessArenaServiceModels.v1
{
    public class PostSubjectSM : SuccessArenaServiceModelBase<int>
    {
        public int PostId { get; set; }

        public int SubjectId { get; set; }
    }
}