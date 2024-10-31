using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace YourBrand.Domain;

public static class TypeBuilderExtensions
{
    public static EntityTypeBuilder AddOrganizationIndex(this EntityTypeBuilder entityTypeBuilder)
    {
        var entityType = entityTypeBuilder.Metadata.ClrType;

        if (entityType.IsAssignableTo(typeof(IHasOrganization)))
        {
            entityTypeBuilder.HasIndex(nameof(IHasOrganization.OrganizationId));
        }

        return entityTypeBuilder;
    }
}