using SuccessArenaServiceModels.Foundation.Base.CommonResponseRoot;
using SuccessArenaServiceModels.Foundation.Base.Enums;

namespace SuccessArenaFoundation.AutoMapperBindings
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class IgnorePropertyOnReadAttribute : AutoInjectRootAttribute
    {
        public AutoMapConversionType ConversionType { get; set; }

        public IgnorePropertyOnReadAttribute(AutoMapConversionType conversionType = AutoMapConversionType.All)
        {
            ConversionType = conversionType;
        }
    }
}
