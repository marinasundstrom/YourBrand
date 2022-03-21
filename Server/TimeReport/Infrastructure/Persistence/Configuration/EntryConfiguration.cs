
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourCompany.TimeReport.Domain.Entities;

namespace YourCompany.TimeReport.Infrastructure.Persistence.Configurations;

public class EntryConfiguration : IEntityTypeConfiguration<Entry>
{
    public void Configure(EntityTypeBuilder<Entry> builder)
    {
        builder.ToTable("Entries", t => t.IsTemporal());

        builder.Property(x => x.Date)
                .HasConversion(x => x.ToDateTime(TimeOnly.Parse("01:00")), x => DateOnly.FromDateTime(x));

        builder.Ignore(e => e.DomainEvents);

        builder.HasOne(x => x.CreatedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.LastModifiedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);
    }
}