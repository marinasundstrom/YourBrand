using YourCompany.Application.Common.Interfaces;

namespace YourCompany.Infrastructure.Services;

class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}