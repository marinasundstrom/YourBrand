using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Meetings.Domain.Entities;

namespace YourBrand.Meetings.Infrastructure.Persistence.Configurations;

public sealed class MeetingAttendeeConfiguration : IEntityTypeConfiguration<MeetingAttendee>
{
    public void Configure(EntityTypeBuilder<MeetingAttendee> builder)
    {
        builder.ToTable("MeetingAttendees");

        builder.HasKey(x => new { x.OrganizationId, x.MeetingId, x.Id });

        builder.HasIndex(x => x.TenantId);

        builder.HasOne(x => x.Role)
               .WithMany()
               .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(x => x.Functions)
               .WithOne(x => x.MeetingAttendee)
               .HasForeignKey(x => new { x.OrganizationId, x.MeetingId, x.MeetingAttendeeId })
               .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Role).AutoInclude();
        builder.Navigation(x => x.Functions).AutoInclude();

        builder.HasOne(x => x.CreatedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.LastModifiedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

        builder.Ignore(e => e.DomainEvents);
    }
}