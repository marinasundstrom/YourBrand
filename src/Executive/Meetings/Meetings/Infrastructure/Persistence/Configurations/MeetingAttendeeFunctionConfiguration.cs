using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Meetings.Domain.Entities;

namespace YourBrand.Meetings.Infrastructure.Persistence.Configurations;

public sealed class MeetingAttendeeFunctionConfiguration : IEntityTypeConfiguration<MeetingAttendeeFunction>
{
    public void Configure(EntityTypeBuilder<MeetingAttendeeFunction> builder)
    {
        builder.ToTable("MeetingAttendeeFunctions");

        builder.HasKey(x => new { x.OrganizationId, x.MeetingId, x.MeetingAttendeeId, x.Id });

        builder.HasIndex(x => x.TenantId);

        builder.HasOne(x => x.MeetingAttendee)
            .WithMany(x => x.Functions)
            .HasForeignKey(x => new { x.OrganizationId, x.MeetingId, x.MeetingAttendeeId })
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Function)
            .WithMany()
            .HasForeignKey(x => x.MeetingFunctionId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Navigation(x => x.Function).AutoInclude();

        builder.HasOne(x => x.CreatedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.LastModifiedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

        builder.Ignore(e => e.DomainEvents);
    }
}
