using SuccessArenaServiceModels.Foundation.Base;

namespace SuccessArenaServiceModels.v1
{
    public class ExamPostSM : SuccessArenaServiceModelBase<int>
    {
        public int ExamId { get; set; }

        public int PostId { get; set; }
    }
}