using YourBrand.Payments.Application.Common.Interfaces;

namespace YourBrand.Payments.Infrastructure.Services;

class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}