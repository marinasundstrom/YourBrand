using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace YourBrand.Minutes.Infrastructure.Persistence.Configurations;

public sealed class MinutesItemConfiguration : IEntityTypeConfiguration<MinutesItem>
{
    public void Configure(EntityTypeBuilder<MinutesItem> builder)
    {
        builder.ToTable("MinutesItems");

        builder.HasKey(x => new { x.OrganizationId, x.Id });

        builder.HasIndex(x => x.TenantId);

        builder.HasOne(x => x.CreatedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.LastModifiedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

        builder.Ignore(e => e.DomainEvents);
    }
}
