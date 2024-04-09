using YourBrand.ApiKeys.Application.Common.Interfaces;

namespace YourBrand.ApiKeys.Infrastructure.Services;

sealed class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}