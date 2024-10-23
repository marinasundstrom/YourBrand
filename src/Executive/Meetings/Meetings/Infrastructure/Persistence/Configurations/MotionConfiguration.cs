using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace YourBrand.Agendas.Infrastructure.Persistence.Configurations;

public sealed class MotionConfiguration : IEntityTypeConfiguration<Motion>
{
    public void Configure(EntityTypeBuilder<Motion> builder)
    {
        builder.ToTable("Motion");

        builder.HasKey(x => new { x.OrganizationId, x.Id });

        builder.HasIndex(x => x.TenantId);

        builder.HasMany(x => x.OperativeClauses)
            .WithOne()
            .HasForeignKey(x => new { x.OrganizationId, x.MotionId });

        builder.Navigation(x => x.OperativeClauses).AutoInclude();

        //builder.HasMany(x => x.Votes);
        //builder.Navigation(x => x.Votes).AutoInclude();

        builder.HasOne(x => x.CreatedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.LastModifiedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

        builder.Ignore(e => e.DomainEvents);
    }
}