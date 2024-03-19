using YourBrand.IdentityManagement.Application.Common.Interfaces;

namespace YourBrand.IdentityManagement.Infrastructure.Services;

class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}