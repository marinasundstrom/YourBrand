
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Sales.Domain.Entities;

namespace YourBrand.Sales.Persistence.Configurations;

public class SubscriptionPlanConfiguration : IEntityTypeConfiguration<SubscriptionPlan>
{
    public void Configure(EntityTypeBuilder<SubscriptionPlan> builder)
    {
        builder.ToTable("SubscriptionPlans");

        builder.HasIndex(x => x.TenantId);

        builder.Ignore(e => e.DomainEvents);

        builder.HasQueryFilter(e => e.Deleted == null);
    }
}