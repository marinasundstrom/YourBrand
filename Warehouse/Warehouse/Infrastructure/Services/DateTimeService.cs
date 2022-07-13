using YourBrand.Warehouse.Application.Common.Interfaces;

namespace YourBrand.Warehouse.Infrastructure.Services;

class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}