using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using YourBrand.Tenancy;

namespace YourBrand.TimeReport.Infrastructure.Persistence;

internal sealed class TenantIdConverter : ValueConverter<TenantId, string>
{
    public TenantIdConverter()
        : base(v => v.Value, v => new(v))
    {
    }
}