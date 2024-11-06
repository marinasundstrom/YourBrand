
using LinqKit;

using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace YourBrand;

public class DomainModelConfigurator
{
    public DomainModelConfigurator()
    {
        QueryFilters = new QueryFilterCollection();
    }

    public IQueryFilterCollection QueryFilters { get; }

    public DomainModelConfiguration Build() 
    {
        return new DomainModelConfiguration(QueryFilters.ToList());
    }
}

public sealed class DomainModelConfiguration 
{
    public DomainModelConfiguration(List<IQueryFilter> queryFilters)
    {
        QueryFilters = queryFilters;
    }

    public List<IQueryFilter> QueryFilters { get; }

    public void ConfigureEntityType(EntityTypeBuilder entityTypeBuilder)
    {
        var entityType = entityTypeBuilder.Metadata.ClrType;

        var queryFilterBuilder = new QueryFilterBuilder(entityType);
        
        this.QueryFilters
            .ForEach(qf =>
            {
                if (!qf.CanApplyTo(entityType))
                    return;

                queryFilterBuilder.Add(qf);
            });

        var filterExpr = queryFilterBuilder.Build();

        if (filterExpr is not null)
        {
            entityTypeBuilder.HasQueryFilter(filterExpr);
        }
    }
}
