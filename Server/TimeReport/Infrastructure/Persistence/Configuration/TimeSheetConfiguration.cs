
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using TimeReport.Domain.Entities;

namespace TimeReport.Infrastructure.Persistence.Configurations;

public class TimeSheetConfiguration : IEntityTypeConfiguration<TimeSheet>
{
    public void Configure(EntityTypeBuilder<TimeSheet> builder)
    {
        builder.ToTable("TimeSheets", t => t.IsTemporal());
        builder.HasQueryFilter(i => i.Deleted == null);
    }
}