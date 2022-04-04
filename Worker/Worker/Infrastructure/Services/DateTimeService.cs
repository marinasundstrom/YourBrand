using Worker.Application.Common.Interfaces;

namespace Worker.Infrastructure.Services;

class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}