using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using YourBrand.Catalog.Domain.Entities;

namespace YourBrand.Catalog.Persistence.Configurations;

public class SubscriptionPlanConfiguration : IEntityTypeConfiguration<ProductSubscriptionPlan>
{
    public void Configure(EntityTypeBuilder<ProductSubscriptionPlan> builder)
    {
        builder.ToTable("SubscriptionPlans");

        builder.HasKey(x => new { x.OrganizationId, x.Id });

        builder.OwnsOne(s => s.Trial);

        builder.Property(p => p.CancellationFinalizationPeriod).HasConversion(new TimeSpanToTicksConverter());
    }
}