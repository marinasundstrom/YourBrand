using YourBrand.Customers.Application.Common.Interfaces;

namespace YourBrand.Customers.Infrastructure.Services;

class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}