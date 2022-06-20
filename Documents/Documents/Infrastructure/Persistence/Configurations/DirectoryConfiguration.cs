
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Directory = YourBrand.Documents.Domain.Entities.Directory;

namespace YourBrand.Documents.Infrastructure.Persistence.Configurations;

public class DirectoryConfiguration : IEntityTypeConfiguration<Directory>
{
    public void Configure(EntityTypeBuilder<Directory> builder)
    {
        builder.ToTable("Directories");

        builder
            .HasMany(x => x.Directories)
            .WithOne(x => x.Parent)
            .OnDelete(DeleteBehavior.NoAction)
            .HasForeignKey(x => x.ParentId);

        builder
            .HasMany(x => x.Documents)
            .WithOne(x => x.Directory)
            .OnDelete(DeleteBehavior.Cascade)
            .HasForeignKey(x => x.DirectoryId);

        builder.HasQueryFilter(x => x.Deleted == null);
    }
}