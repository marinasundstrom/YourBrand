
using YourCompany.Showroom.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace YourCompany.Showroom.Infrastructure.Persistence.Configurations;

class ConsultantProfilCaseConfigurationConfiguration : IEntityTypeConfiguration<ConsultantProfile>
{
    public void Configure(EntityTypeBuilder<ConsultantProfile> builder)
    {
        builder.ToTable("ConsultantProfiles");
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.HasQueryFilter(i => i.Deleted == null);

        builder
            .HasMany(p => p.Skills)
            .WithMany(p => p.ConsultantProfiles)
            .UsingEntity<ConsultantProfileSkill>(
                j => j
                    .HasOne(pt => pt.Skill)
                    .WithMany(p => p.ConsultantProfileSkills)
                    .HasForeignKey(pt => pt.SkillId)
                    .OnDelete(DeleteBehavior.NoAction),
                j => j
                    .HasOne(pt => pt.ConsultantProfile)
                    .WithMany(t => t.ConsultantProfileSkills)
                    .HasForeignKey(pt => pt.ConsultantProfileId)
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