using YourBrand.TimeReport.Application.Common.Interfaces;

namespace YourBrand.TimeReport.Infrastructure.Services;

class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}