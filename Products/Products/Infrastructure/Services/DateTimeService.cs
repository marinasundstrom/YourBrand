using YourBrand.Products.Application.Common.Interfaces;

namespace YourBrand.Products.Infrastructure.Services;

class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}