using YourBrand.Documents.Application.Common.Interfaces;

namespace YourBrand.Documents.Infrastructure.Services;

class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}