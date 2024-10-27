using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Sales.Domain.Entities;

namespace YourBrand.Sales.Persistence.Configurations;

public sealed class SubscriptionTypeConfiguration : IEntityTypeConfiguration<SubscriptionType>
{
    public void Configure(EntityTypeBuilder<SubscriptionType> builder)
    {
        builder.ToTable("SubscriptionTypes");

        builder.HasKey(x => new { x.OrganizationId, x.Id });

        //builder.HasAlternateKey(o => new { o.OrganizationId, o.Id });

        builder.HasIndex(x => x.TenantId);

        builder.Ignore(e => e.DomainEvents);

    }
}