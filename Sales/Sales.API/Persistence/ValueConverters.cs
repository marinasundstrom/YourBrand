using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using YourBrand.Sales.API.Features.OrderManagement.Domain.ValueObjects;

namespace YourBrand.Sales.API.Persistence;
internal sealed class TenantIdConverter : ValueConverter<TenantId, string>
{
    public TenantIdConverter()
        : base(v => v.Value, v => new(v))
    {
    }
}