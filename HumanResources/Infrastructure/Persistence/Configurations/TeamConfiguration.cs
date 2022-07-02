using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.HumanResources.Domain.Entities;

namespace YourBrand.HumanResources.Infrastructure.Persistence.Configurations;

public class TeamConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.ToTable(name: "Teams");

        builder.HasMany(p => p.Members)
            .WithMany(p => p.Teams)
            .UsingEntity<TeamMembership>(
                j => j
                    .HasOne(pt => pt.Person)
                    .WithMany(t => t.TeamMemberships)
                    .HasForeignKey(pt => pt.PersonId),

                j => j
                    .HasOne(pt => pt.Team)
                    .WithMany(p => p.Memberships)
                    .HasForeignKey(pt => pt.TeamId));

    }
}
