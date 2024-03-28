using YourBrand.Application.Common.Interfaces;

namespace YourBrand.Infrastructure.Services;

class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}