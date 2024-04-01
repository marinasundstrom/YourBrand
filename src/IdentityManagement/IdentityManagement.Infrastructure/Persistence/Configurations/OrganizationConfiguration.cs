using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.IdentityManagement.Domain.Entities;

namespace YourBrand.IdentityManagement.Infrastructure.Persistence.Configurations;

public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.ToTable(name: "Organizations");

        builder.HasIndex(x => x.TenantId);

        builder.HasMany(p => p.Users)
            .WithMany(p => p.Organizations)
            .UsingEntity<OrganizationUser>(
                j => j
                    .HasOne(pt => pt.User)
                    .WithMany(t => t.OrganizationUsers)
                    .HasForeignKey(pt => pt.UserId),

                j => j
                    .HasOne(pt => pt.Organization)
                    .WithMany(p => p.OrganizationUsers)
                    .HasForeignKey(pt => pt.OrganizationId));
    }
}