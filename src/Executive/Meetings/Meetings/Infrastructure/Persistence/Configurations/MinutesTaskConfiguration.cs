using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Meetings.Domain.Entities;

namespace YourBrand.Meetings.Infrastructure.Persistence.Configurations;

public sealed class MinutesTaskConfiguration : IEntityTypeConfiguration<MinutesTask>
{
    public void Configure(EntityTypeBuilder<MinutesTask> builder)
    {
        builder.ToTable("MinutesTasks");

        builder.HasKey(x => new { x.OrganizationId, x.MinutesId, x.Id });

        builder.HasIndex(x => x.OrganizationId);
        builder.HasIndex(x => x.TenantId);

        builder.HasOne(x => x.Minutes)
            .WithMany(x => x.Tasks)
            .HasForeignKey(x => new { x.OrganizationId, x.MinutesId })
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.AssignedToName)
            .HasMaxLength(200);

        builder.Property(x => x.AssignedToEmail)
            .HasMaxLength(200);

        builder.HasOne(x => x.AssignedTo)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.CompletedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.CreatedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.LastModifiedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

        builder.Ignore(x => x.DomainEvents);
    }
}
