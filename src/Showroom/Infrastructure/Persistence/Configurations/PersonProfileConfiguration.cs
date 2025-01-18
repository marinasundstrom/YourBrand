﻿
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Showroom.Domain.Entities;

namespace YourBrand.Showroom.Infrastructure.Persistence.Configurations;

sealed class PersonProfilCaseConfigurationConfiguration : IEntityTypeConfiguration<PersonProfile>
{
    public void Configure(EntityTypeBuilder<PersonProfile> builder)
    {
        builder.ToTable("PersonProfiles");
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.HasQueryFilter(i => i.Deleted == null);

        builder.HasIndex(x => x.TenantId);

        builder
            .HasMany(p => p.Employments)
            .WithOne(x => x.PersonProfile)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasMany(p => p.Assignments)
            .WithOne(x => x.PersonProfile)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasMany(p => p.Projects)
            .WithOne(x => x.PersonProfile)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(p => p.Skills)
            .WithMany(p => p.PersonProfiles)
            .UsingEntity<PersonProfileSkill>(
                j => j
                    .HasOne(pt => pt.Skill)
                    .WithMany(p => p.PersonProfileSkills)
                    .HasForeignKey(pt => pt.SkillId)
                    .OnDelete(DeleteBehavior.NoAction),
                j => j
                    .HasOne(pt => pt.PersonProfile)
                    .WithMany(t => t.PersonProfileSkills)
                    .HasForeignKey(pt => pt.PersonProfileId)
                    .OnDelete(DeleteBehavior.NoAction)
                );

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