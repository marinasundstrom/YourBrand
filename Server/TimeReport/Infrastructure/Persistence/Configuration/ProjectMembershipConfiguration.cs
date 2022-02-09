
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using TimeReport.Domain.Entities;

namespace TimeReport.Infrastructure.Persistence.Configurations;

public class ProjectMembershipConfiguration : IEntityTypeConfiguration<ProjectMembership>
{
    public void Configure(EntityTypeBuilder<ProjectMembership> builder)
    {
        builder.ToTable("ProjectMemberships", t => t.IsTemporal());
        builder.HasQueryFilter(i => i.Deleted == null);
    }
}