using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Sales.Domain.Entities;

namespace YourBrand.Sales.Persistence.Configurations;

public sealed class OrderTypeConfiguration : IEntityTypeConfiguration<OrderType>
{
    public void Configure(EntityTypeBuilder<OrderType> builder)
    {
        builder.ToTable("OrderTypes");

        builder.HasKey(x => new { x.OrganizationId, x.Id });

        //builder.HasAlternateKey(o => new { o.OrganizationId, o.Id });

        builder.HasIndex(x => x.TenantId);

        builder.Ignore(e => e.DomainEvents);

    }
}