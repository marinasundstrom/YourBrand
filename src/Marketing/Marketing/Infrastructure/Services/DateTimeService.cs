using YourBrand.Marketing.Application.Common.Interfaces;

namespace YourBrand.Marketing.Infrastructure.Services;

class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}