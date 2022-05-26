using YourBrand.Invoices.Application.Common.Interfaces;

namespace YourBrand.Invoices.Infrastructure.Services;

class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}