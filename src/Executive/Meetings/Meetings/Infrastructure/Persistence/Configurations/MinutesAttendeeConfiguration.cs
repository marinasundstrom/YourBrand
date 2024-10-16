using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace YourBrand.Minutess.Infrastructure.Persistence.Configurations;

public sealed class MinutesAttendeeConfiguration : IEntityTypeConfiguration<MinutesAttendee>
{
    public void Configure(EntityTypeBuilder<MinutesAttendee> builder)
    {
        builder.ToTable("MinutesAttendees");

        builder.HasKey(x => new { x.OrganizationId, x.MinutesId, x.Id });

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