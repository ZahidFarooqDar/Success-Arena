using SuccessArenaServiceModels.Foundation.Base.CommonResponseRoot;

namespace SuccessArenaFoundation.AutoMapperBindings
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class IgnoreClassAutoMapAttribute : AutoInjectRootAttribute
    {
    }
}
