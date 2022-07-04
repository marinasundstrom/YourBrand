using YourBrand.Orders.Application.Common.Interfaces;

namespace YourBrand.Orders.Infrastructure.Services;

class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}