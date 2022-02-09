using Skynet.TimeReport.Application.Common.Interfaces;

namespace Skynet.TimeReport.Infrastructure.Services;

class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}