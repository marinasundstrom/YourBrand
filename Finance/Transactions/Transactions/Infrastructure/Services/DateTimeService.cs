using Transactions.Application.Common.Interfaces;

namespace Transactions.Infrastructure.Services;

class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}