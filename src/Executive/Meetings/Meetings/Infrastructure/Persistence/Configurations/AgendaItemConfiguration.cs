using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace YourBrand.Agendas.Infrastructure.Persistence.Configurations;

public sealed class AgendaItemConfiguration : IEntityTypeConfiguration<AgendaItem>
{
    public void Configure(EntityTypeBuilder<AgendaItem> builder)
    {
        builder.ToTable("AgendaItems");

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
