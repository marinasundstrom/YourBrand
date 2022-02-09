using TimeReport.Application.Common.Interfaces;

namespace TimeReport.Infrastructure.Services;

class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}