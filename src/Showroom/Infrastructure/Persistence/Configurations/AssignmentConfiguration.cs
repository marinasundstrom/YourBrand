
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Showroom.Domain.Entities;

namespace YourBrand.Showroom.Infrastructure.Persistence.Configurations;

sealed class AssignmentConfiguration : IEntityTypeConfiguration<Assignment>
{
    public void Configure(EntityTypeBuilder<Assignment> builder)
    {
        builder.ToTable("Assignments");

        builder.HasIndex(x => x.TenantId);

        builder
            .HasMany(p => p.Roles)
            .WithOne(x => x.Assignment)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(p => p.Employment)
            .WithMany(x => x.Assignments)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasOne(p => p.PersonProfile)
            .WithMany(x => x.Assignments)
            .OnDelete(DeleteBehavior.NoAction); 
    }
}
