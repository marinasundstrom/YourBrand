using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace YourBrand.Tenancy;

public sealed class TenantIdConverter : ValueConverter<TenantId, string>
{
    public TenantIdConverter()
        : base(v => v.Value, v => new(v))
    {
    }
}