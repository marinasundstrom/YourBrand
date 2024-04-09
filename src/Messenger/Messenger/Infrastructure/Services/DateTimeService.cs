using YourBrand.Messenger.Application.Common.Interfaces;

namespace YourBrand.Messenger.Infrastructure.Services;

sealed class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}