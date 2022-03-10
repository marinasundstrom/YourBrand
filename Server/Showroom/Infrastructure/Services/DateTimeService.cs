using Skynet.Showroom.Application.Common.Interfaces;

namespace Skynet.Showroom.Infrastructure.Services;

class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}