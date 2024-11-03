using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Accounting.Domain.Entities;

namespace YourBrand.Accounting.Infrastructure.Persistence.Configurations;

public class LedgerEntryConfiguration : IEntityTypeConfiguration<LedgerEntry>
{
    public void Configure(EntityTypeBuilder<LedgerEntry> builder)
    {
        builder.ToTable("LedgerEntries");

        builder.HasKey(x => new { x.OrganizationId, x.Id });

        builder.HasIndex(x => x.TenantId);

        builder.HasOne(x => x.Account)
            .WithMany(x => x.Entries)
            .HasForeignKey(x => new { x.OrganizationId, x.AccountNo })
            .OnDelete(DeleteBehavior.NoAction);
    }
}