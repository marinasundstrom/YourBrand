
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using TimeReport.Domain.Entities;

namespace TimeReport.Infrastructure.Persistence.Configurations;

public class TimeSheetActivityConfiguration : IEntityTypeConfiguration<TimeSheetActivity>
{
    public void Configure(EntityTypeBuilder<TimeSheetActivity> builder)
    {
        builder.ToTable("TimeSheetActivities", t => t.IsTemporal());
        builder.HasQueryFilter(i => i.Deleted == null);
    }
}