using YourBrand.UserManagement.Application.Common.Interfaces;

namespace YourBrand.UserManagement.Infrastructure.Services;

class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}