using YourBrand.Invoicing.Application.Common.Interfaces;

namespace YourBrand.Invoicing.Infrastructure.Services;

sealed class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}