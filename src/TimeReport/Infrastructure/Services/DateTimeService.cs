using YourBrand.TimeReport.Application.Common.Interfaces;

namespace YourBrand.TimeReport.Infrastructure.Services;

sealed class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}