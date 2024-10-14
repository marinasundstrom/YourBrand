using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace YourBrand.MeetingGroups.Infrastructure.Persistence.Configurations;

public sealed class MeetingGroupMemberConfiguration : IEntityTypeConfiguration<MeetingGroupMember>
{
    public void Configure(EntityTypeBuilder<MeetingGroupMember> builder)
    {
        builder.ToTable("MeetingGroupMembers");

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
