using YourBrand.RotRutService.Application.Common.Interfaces;

namespace YourBrand.RotRutService.Infrastructure.Services;

class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}