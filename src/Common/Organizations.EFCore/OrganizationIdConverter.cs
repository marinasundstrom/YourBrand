using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace YourBrand.Domain;

public sealed class OrganizationIdConverter : ValueConverter<OrganizationId, string>
{
    public OrganizationIdConverter()
        : base(v => v.Value, v => new(v))
    {
    }
}