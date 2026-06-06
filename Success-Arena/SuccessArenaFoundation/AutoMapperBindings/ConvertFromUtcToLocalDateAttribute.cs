using SuccessArenaServiceModels.Foundation.Base.CommonResponseRoot;

namespace SuccessArenaFoundation.AutoMapperBindings
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class ConvertFromUtcToLocalDateAttribute : AutoInjectRootAttribute
    {
        public string? SourcePropertyName { get; set; }
    }
}
