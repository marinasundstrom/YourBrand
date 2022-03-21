using YourCompany.IdentityService.Application.Common.Interfaces;

namespace YourCompany.IdentityService.Infrastructure.Services;

class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}