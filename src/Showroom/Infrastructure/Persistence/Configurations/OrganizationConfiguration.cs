
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Showroom.Domain.Entities;

namespace YourBrand.Showroom.Infrastructure.Persistence.Configurations;

sealed class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.ToTable("Organizations");
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.HasQueryFilter(i => i.Deleted == null);

        builder.HasIndex(x => x.TenantId);

        builder.OwnsOne(x => x.Address);

        builder.HasMany(p => p.Users)
            .WithMany(p => p.Organization)
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

        builder.HasOne(x => x.DeletedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);
    }
}
