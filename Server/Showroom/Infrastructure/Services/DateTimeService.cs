using YourCompany.Showroom.Application.Common.Interfaces;

namespace YourCompany.Showroom.Infrastructure.Services;

class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}