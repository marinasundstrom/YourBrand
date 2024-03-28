using YourBrand.Accounting.Application.Common.Interfaces;

namespace YourBrand.Accounting.Infrastructure.Services;

class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}