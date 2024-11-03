using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Accounting.Domain.Entities;

namespace YourBrand.Accounting.Infrastructure.Persistence.Configurations;

public class JournalEntryConfiguration : IEntityTypeConfiguration<JournalEntry>
{
    public void Configure(EntityTypeBuilder<JournalEntry> builder)
    {
        builder.ToTable("JournalEntries");

        builder.HasKey(x => new { x.OrganizationId, x.Id });

        builder.HasIndex(x => x.TenantId);

        builder.HasMany(x => x.Entries)
            .WithOne(x => x.JournalEntry)
            .HasForeignKey(x => new { x.OrganizationId, x.JournalEntryId });

        builder.HasMany(x => x.Verifications)
            .WithOne(x => x.JournalEntry)
            .HasForeignKey(x => new { x.OrganizationId, x.JournalEntryId });
    }
}