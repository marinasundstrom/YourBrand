using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace YourBrand.Meetings.Infrastructure.Persistence.Configurations;

public sealed class MinuteConfiguration : IEntityTypeConfiguration<Meetings.Domain.Entities.Minutes>
{
    public void Configure(EntityTypeBuilder<Meetings.Domain.Entities.Minutes> builder)
    {
        builder.ToTable("Minutes");

        builder.HasKey(x => new { x.OrganizationId, x.Id });

        builder.HasIndex(x => x.TenantId);

        builder.HasMany(x => x.Attendees)
            .WithOne()
            .HasForeignKey(x => new { x.OrganizationId, x.MinutesId });

        builder.Navigation(x => x.Attendees).AutoInclude();

        builder.HasOne(x => x.Agenda)
            .WithOne()
            .HasForeignKey<Agenda>(x => new { x.OrganizationId, x.MinutesId });

        builder.Navigation(x => x.Agenda).AutoInclude();

        builder.HasMany(x => x.Items)
            .WithOne()
            .HasForeignKey(x => new { x.OrganizationId, x.MinutesId });

        builder.Navigation(x => x.Items).AutoInclude();

        builder.HasMany(x => x.Tasks)
            .WithOne()
            .HasForeignKey(x => new { x.OrganizationId, x.MinutesId });

        builder.Navigation(x => x.Tasks).AutoInclude();

        builder.HasOne(x => x.CreatedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.LastModifiedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

        builder.Ignore(e => e.DomainEvents);
    }
}