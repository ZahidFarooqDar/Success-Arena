using SuccessArenaServiceModels.Foundation.Base;

namespace SuccessArenaServiceModels.v1
{
    public class ExamSM : SuccessArenaServiceModelBase<int>
    {
        public string Name { get; set; }

        public string Code { get; set; } 

        public string Description { get; set; }

        public string IconUrl { get; set; }

        public bool IsActive { get; set; }
    }
}
