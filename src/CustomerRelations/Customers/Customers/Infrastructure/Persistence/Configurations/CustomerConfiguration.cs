using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Customers.Domain.Entities;

namespace YourBrand.Customers.Infrastructure.Persistence.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder
            .UseTpcMappingStrategy()
            .Property(e => e.Id).HasDefaultValueSql("NEXT VALUE FOR [CustomerIds]");

        builder.HasIndex(x => x.TenantId);
    }
}