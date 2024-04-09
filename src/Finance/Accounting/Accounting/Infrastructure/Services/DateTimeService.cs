using YourBrand.Accounting.Application.Common.Interfaces;

namespace YourBrand.Accounting.Infrastructure.Services;

sealed class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}