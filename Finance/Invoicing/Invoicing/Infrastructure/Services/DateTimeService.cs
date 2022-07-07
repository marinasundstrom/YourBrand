using YourBrand.Invoicing.Application.Common.Interfaces;

namespace YourBrand.Invoicing.Infrastructure.Services;

class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}