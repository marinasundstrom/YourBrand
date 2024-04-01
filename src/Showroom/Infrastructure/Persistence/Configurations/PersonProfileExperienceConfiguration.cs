
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Showroom.Domain.Entities;

namespace YourBrand.Showroom.Infrastructure.Persistence.Configurations;

class PersonProfileExperienceConfiguration : IEntityTypeConfiguration<PersonProfileExperience>
{
    public void Configure(EntityTypeBuilder<PersonProfileExperience> builder)
    {
        builder.ToTable("PersonProfileExperiences");
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.HasQueryFilter(i => i.Deleted == null);

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