
using Skynet.Showroom.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Skynet.Showroom.Infrastructure.Persistence.Configurations;

class CompetenceAreaConfiguration : IEntityTypeConfiguration<CompetenceArea>
{
    public void Configure(EntityTypeBuilder<CompetenceArea> builder)
    {
        builder.ToTable("CompetenceAreas");
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
