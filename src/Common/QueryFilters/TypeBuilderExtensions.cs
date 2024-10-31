using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace YourBrand;

public static class TypeBuilderExtensions
{
    public static EntityTypeBuilder RegisterQueryFilters(this EntityTypeBuilder entityTypeBuilder, Action<QueryFilterBuilder> config)
    {
        var entityType = entityTypeBuilder.Metadata.ClrType;

        var queryFilterBuilder = new QueryFilterBuilder(entityType);

        config(queryFilterBuilder);

        var queryFilter = queryFilterBuilder.Build();
        if (queryFilter is not null)
        {
            entityTypeBuilder.HasQueryFilter(queryFilter);
        }

        return entityTypeBuilder;
    }
}