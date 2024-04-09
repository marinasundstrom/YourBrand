using YourBrand.Documents.Application.Common.Interfaces;

namespace YourBrand.Documents.Infrastructure.Services;

sealed class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}