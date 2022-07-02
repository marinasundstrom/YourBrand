using YourBrand.HumanResources.Application.Common.Interfaces;

namespace YourBrand.HumanResources.Infrastructure.Services;

class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}