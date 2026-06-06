using AutoMapper;
using SuccessArenaBAL.Foundation.CommonUtils;

namespace SuccessArenaFoundation.AutoMapperBindings
{
    public class FilePathToUrlConverter : IValueConverter<string, string>
    {
        public string Convert(string sourceMember, ResolutionContext context)
        {
            return sourceMember.ConvertFromFilePathToUrl();
        }
    }
}
