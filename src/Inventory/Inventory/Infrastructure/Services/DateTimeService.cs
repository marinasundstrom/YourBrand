using YourBrand.Inventory.Application.Common.Interfaces;

namespace YourBrand.Inventory.Infrastructure.Services;

class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}