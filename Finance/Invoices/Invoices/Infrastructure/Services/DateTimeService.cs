using Invoices.Application.Common.Interfaces;

namespace Invoices.Infrastructure.Services;

class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}