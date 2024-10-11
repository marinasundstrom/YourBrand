using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace YourBrand.Agendas.Infrastructure.Persistence.Configurations;

public sealed class OperativeClauseConfiguration : IEntityTypeConfiguration<MotionOperativeClause>
{
    public void Configure(EntityTypeBuilder<MotionOperativeClause> builder)
    {
        builder.ToTable("MotionOperativeClauses");

        builder.HasKey(x => new { x.OrganizationId, x.Id });

        builder.HasIndex(x => x.TenantId);

        builder.HasOne(x => x.CreatedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.LastModifiedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

        builder.Ignore(e => e.DomainEvents);
    }
}
