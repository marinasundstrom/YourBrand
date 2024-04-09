using YourBrand.Application.Common.Interfaces;

namespace YourBrand.Infrastructure.Services;

sealed class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}