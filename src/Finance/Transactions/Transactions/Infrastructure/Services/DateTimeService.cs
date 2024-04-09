using YourBrand.Transactions.Application.Common.Interfaces;

namespace YourBrand.Transactions.Infrastructure.Services;

sealed class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}