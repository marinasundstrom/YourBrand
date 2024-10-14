using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace YourBrand.MeetingGroups.Infrastructure.Persistence.Configurations;

public sealed class MeetingGroupConfiguration : IEntityTypeConfiguration<MeetingGroup>
{
    public void Configure(EntityTypeBuilder<MeetingGroup> builder)
    {
        builder.ToTable("MeetingGroups");

        builder.HasKey(x => new { x.OrganizationId, x.Id });

        builder.HasIndex(x => x.TenantId);

        builder.HasMany(x => x.Members)
            .WithOne()
            .HasForeignKey(x => new { x.OrganizationId, x.MeetingGroupId });

        builder.Navigation(x => x.Members).AutoInclude();

        builder.OwnsOne(x => x.Quorum);

        builder.HasOne(x => x.CreatedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.LastModifiedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

        builder.Ignore(e => e.DomainEvents);
    }
}
