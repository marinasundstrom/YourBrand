using YourBrand.Catalog.Application.Common.Interfaces;

namespace YourBrand.Catalog.Infrastructure.Services;

class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}