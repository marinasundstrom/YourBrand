
using YourBrand.Showroom.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace YourBrand.Showroom.Infrastructure.Persistence.Configurations;

class PersonProfileSkillConfiguration : IEntityTypeConfiguration<PersonProfileSkill>
{
    public void Configure(EntityTypeBuilder<PersonProfileSkill> builder)
    {
        builder.ToTable("PersonProfileSkills");
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.HasQueryFilter(i => i.Deleted == null);

        builder.OwnsOne(x => x.Link);

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
