using YourBrand.Marketing.Application.Common.Interfaces;

namespace YourBrand.Marketing.Infrastructure.Services;

sealed class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}