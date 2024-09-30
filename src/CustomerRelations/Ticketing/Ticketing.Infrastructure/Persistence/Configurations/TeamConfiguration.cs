﻿
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace YourBrand.Ticketing.Infrastructure.Persistence.Configurations;

public class TeamConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.ToTable("Teams");

        builder.Ignore(x => x.DomainEvents);

        builder.HasMany(p => p.Members)
            .WithMany(p => p.Teams)
            .UsingEntity<TeamMembership>(
                j => j
                    .HasOne(pt => pt.User)
                    .WithMany(t => t.TeamMemberships)
                    .HasForeignKey(pt => pt.UserId),

                j => j
                    .HasOne(pt => pt.Team)
                    .WithMany(p => p.Memberships)
                    .HasForeignKey(pt => pt.TeamId));

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