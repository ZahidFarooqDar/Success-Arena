using SuccessArenaServiceModels.Foundation.Base.Enums;

namespace SuccessArenaServiceModels.Foundation.Base.CommonResponseRoot
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class IgnorePropertyOnWriteAttribute : AutoInjectRootAttribute
    {
        public AutoMapConversionType ConversionType { get; set; }

        public IgnorePropertyOnWriteAttribute(AutoMapConversionType conversionType = AutoMapConversionType.All)
        {
            ConversionType = conversionType;
        }
    }
}
