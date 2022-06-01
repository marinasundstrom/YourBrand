
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.TimeReport.Domain.Entities;

namespace YourBrand.TimeReport.Infrastructure.Persistence.Configurations;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("Projects");
        builder.HasQueryFilter(i => i.Deleted == null);

       builder.HasMany(p => p.Teams)
            .WithMany(p => p.Projects)
            .UsingEntity<ProjectTeam>(
                j => j
                    .HasOne(pt => pt.Team)
                    .WithMany(t => t.ProjectTeams)
                    .HasForeignKey(pt => pt.TeamId),

                j => j
                    .HasOne(pt => pt.Project)
                    .WithMany(p => p.ProjectTeams)
                    .HasForeignKey(pt => pt.ProjectId));

        builder.HasMany(x => x.ProjectTeams)
            .WithOne(x => x.Project)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.CreatedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.LastModifiedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.DeletedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);
    }
}