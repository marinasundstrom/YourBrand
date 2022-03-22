using YourBrand.IdentityService.Application.Common.Interfaces;

namespace YourBrand.IdentityService.Infrastructure.Services;

class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}