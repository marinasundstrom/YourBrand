using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.IdentityService.Domain.Entities;

namespace YourBrand.IdentityService.Infrastructure.Persistence.Configurations;

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

public class TeamMembershipConfiguration : IEntityTypeConfiguration<TeamMembership>
{
    public void Configure(EntityTypeBuilder<TeamMembership> builder)
    {
        builder.ToTable(name: "TeamMemberships");
    }
}