using Accounting.Application.Common.Interfaces;

namespace Accounting.Infrastructure.Services;

class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}