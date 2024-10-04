using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace YourBrand.Agendas.Infrastructure.Persistence.Configurations;

public sealed class DebateEntryConfiguration : IEntityTypeConfiguration<DebateEntry>
{
    public void Configure(EntityTypeBuilder<DebateEntry> builder)
    {
        builder.ToTable("DebateEntries");

        builder.HasKey(x => new { x.OrganizationId, x.Id });

        builder.HasIndex(x => x.TenantId);

        builder.HasMany<DebateEntry>(x => x.Replies)
            .WithOne()
            .HasForeignKey(x => new { x.OrganizationId, x.ReplyToEntryId })
            .OnDelete(DeleteBehavior.ClientCascade);

        builder.HasOne(x => x.CreatedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.LastModifiedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

        builder.Ignore(e => e.DomainEvents);
    }
}
