using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.IdentityManagement.Domain.Entities;

namespace YourBrand.IdentityManagement.Infrastructure.Persistence.Configurations;

public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable(name: "Tenants");

        builder
            .HasMany(x => x.Organizations)
            .WithOne(x => x.Tenant)
            .OnDelete(DeleteBehavior.ClientCascade);
            
        builder
            .HasMany(x => x.Users)
            .WithOne(x => x.Tenant)
            .OnDelete(DeleteBehavior.ClientCascade);
    }
}
