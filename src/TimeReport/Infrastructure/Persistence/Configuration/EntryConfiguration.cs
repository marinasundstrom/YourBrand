
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.TimeReport.Domain.Entities;

namespace YourBrand.TimeReport.Infrastructure.Persistence.Configurations;

public class EntryConfiguration : IEntityTypeConfiguration<Entry>
{
    public void Configure(EntityTypeBuilder<Entry> builder)
    {
        builder.ToTable("Entries");

        builder.Ignore(x => x.DomainEvents);

        builder.Property(x => x.Date)
                .HasConversion(x => x.ToDateTime(TimeOnly.Parse("01:00")), x => DateOnly.FromDateTime(x));

        builder.HasOne(x => x.CreatedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.LastModifiedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);
    }
}