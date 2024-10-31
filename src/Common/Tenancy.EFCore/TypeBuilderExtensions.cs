using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace YourBrand.Tenancy;

public static class TypeBuilderExtensions
{
    public static EntityTypeBuilder AddTenantIndex(this EntityTypeBuilder entityTypeBuilder)
    {
        var entityType = entityTypeBuilder.Metadata.ClrType;

        if (entityType.IsAssignableTo(typeof(IHasTenant)))
        {
            entityTypeBuilder.HasIndex(nameof(IHasTenant.TenantId));
        }

        return entityTypeBuilder;
    }
}
