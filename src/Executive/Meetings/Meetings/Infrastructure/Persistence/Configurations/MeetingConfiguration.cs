using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace YourBrand.Meetings.Infrastructure.Persistence.Configurations;

public sealed class MeetingConfiguration : IEntityTypeConfiguration<Meeting>
{
    public void Configure(EntityTypeBuilder<Meeting> builder)
    {
        builder.ToTable("Meetings");

        builder.HasKey(x => new { x.OrganizationId, x.Id });

        builder.HasIndex(x => x.TenantId);

        builder.HasMany(x => x.Attendees)
            .WithOne()
            .HasForeignKey(x => new { x.OrganizationId, x.MeetingId })
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Attendees).AutoInclude();

        builder.HasOne(x => x.JoinAs)
            .WithMany()
            .HasForeignKey(x => x.JoinAsId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Navigation(x => x.JoinAs).AutoInclude();

        builder.HasOne(x => x.Agenda)
            .WithOne()
            .HasForeignKey<Agenda>(x => new { x.OrganizationId, x.MeetingId });

        builder.Navigation(x => x.Agenda).AutoInclude();

        builder.HasOne(x => x.Minutes)
            .WithOne()
            .HasForeignKey<Domain.Entities.Minutes>(x => new { x.OrganizationId, x.MeetingId });

        builder.Navigation(x => x.Minutes).AutoInclude();

        builder.OwnsOne(x => x.Quorum);

        builder.Property(x => x.AdjournmentMessage)
            .HasMaxLength(1024);

        builder.HasOne(x => x.CreatedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.LastModifiedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

        builder.Ignore(e => e.DomainEvents);
    }
}