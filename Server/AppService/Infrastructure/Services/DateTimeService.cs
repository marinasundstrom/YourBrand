using Skynet.Application.Common.Interfaces;

namespace Skynet.Infrastructure.Services;

class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}