using AutoMapper;

namespace SuccessArenaFoundation.AutoMapperBindings
{
    public class LocalDateTimeValueConverter : IValueConverter<DateTime, DateTime>
    {
        public DateTime Convert(DateTime sourceMember, ResolutionContext context)
        {
            return sourceMember.ConvertFromUTCToSystemTimezone();
        }
    }
}
