
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

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

        builder.OwnsOne(s => s.Schedule);

        builder.OwnsOne(s => s.Trial);

        builder.Property(p => p.CancellationFinalizationPeriod).HasConversion(new TimeSpanToTicksConverter());
    }
}