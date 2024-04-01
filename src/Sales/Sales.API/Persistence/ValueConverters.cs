using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using YourBrand.Sales.Domain.ValueObjects;

namespace YourBrand.Sales.Persistence;
internal sealed class TenantIdConverter : ValueConverter<TenantId, string>
{
    public TenantIdConverter()
        : base(v => v.Value, v => new(v))
    {
    }
}