using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Domain;

namespace YourBrand.Auditability;

public static class TypeBuilderExtensions
{
    public static EntityTypeBuilder AddSoftDeleteIndex(this EntityTypeBuilder entityTypeBuilder)
    {
        var entityType = entityTypeBuilder.Metadata.ClrType;

        if (entityType.IsAssignableTo(typeof(ISoftDeletable)))
        {
            entityTypeBuilder.HasIndex(nameof(ISoftDeletable.IsDeleted));
        }

        return entityTypeBuilder;
    }
}