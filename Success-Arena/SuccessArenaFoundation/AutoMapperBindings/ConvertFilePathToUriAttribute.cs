using SuccessArenaServiceModels.Foundation.Base.CommonResponseRoot;

namespace SuccessArenaFoundation.AutoMapperBindings
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class ConvertFilePathToUriAttribute : AutoInjectRootAttribute
    {
        public string? SourcePropertyName { get; set; }
    }
}
