using YourBrand.RotRutService.Application.Common.Interfaces;

namespace YourBrand.RotRutService.Infrastructure.Services;

sealed class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}