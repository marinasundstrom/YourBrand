using Skynet.IdentityService.Application.Common.Interfaces;

namespace Skynet.IdentityService.Infrastructure.Services;

class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}