using YourBrand.Showroom.Application.Common.Interfaces;

namespace YourBrand.Showroom.Infrastructure.Services;

class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}