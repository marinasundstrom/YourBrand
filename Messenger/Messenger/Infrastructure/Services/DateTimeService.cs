using YourBrand.Messenger.Application.Common.Interfaces;

namespace YourBrand.Messenger.Infrastructure.Services;

class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}