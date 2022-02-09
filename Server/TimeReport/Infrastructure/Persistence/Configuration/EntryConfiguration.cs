
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using TimeReport.Domain.Entities;

namespace TimeReport.Infrastructure.Persistence.Configurations;

public class EntryConfiguration : IEntityTypeConfiguration<Entry>
{
    public void Configure(EntityTypeBuilder<Entry> builder)
    {
        builder.ToTable("Entries", t => t.IsTemporal());

        builder.Property(x => x.Date)
                .HasConversion(x => x.ToDateTime(TimeOnly.Parse("01:00")), x => DateOnly.FromDateTime(x));

        builder.Ignore(e => e.DomainEvents);
    }
}