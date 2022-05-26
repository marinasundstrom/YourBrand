using Documents.Application.Common.Interfaces;

namespace Documents.Infrastructure.Services;

class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}