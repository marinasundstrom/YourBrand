using YourCompany.TimeReport.Application.Common.Interfaces;

namespace YourCompany.TimeReport.Infrastructure.Services;

class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}