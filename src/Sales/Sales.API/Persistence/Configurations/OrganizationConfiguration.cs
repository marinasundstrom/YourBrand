using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Sales.Features.OrderManagement.Domain.Entities;

namespace YourBrand.Sales.Persistence.Configurations;

public sealed class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.ToTable("Organizations");

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

        builder.HasOne(x => x.CreatedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.LastModifiedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

        builder.Ignore(e => e.DomainEvents);

    }
}
