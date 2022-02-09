using Catalog.Application.Common.Interfaces;

namespace Catalog.Infrastructure.Services;

class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}