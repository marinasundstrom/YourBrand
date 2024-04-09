using YourBrand.Showroom.Application.Common.Interfaces;

namespace YourBrand.Showroom.Infrastructure.Services;

sealed class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}