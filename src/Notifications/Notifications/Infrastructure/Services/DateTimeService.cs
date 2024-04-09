using YourBrand.Notifications.Application.Common.Interfaces;

namespace YourBrand.Notifications.Infrastructure.Services;

sealed class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}