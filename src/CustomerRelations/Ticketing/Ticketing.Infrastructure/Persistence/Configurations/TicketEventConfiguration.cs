using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace YourBrand.Ticketing.Infrastructure.Persistence.Configurations;

public sealed class TicketEventConfiguration : IEntityTypeConfiguration<TicketEvent>
{
    public void Configure(EntityTypeBuilder<TicketEvent> builder)
    {
        builder.ToTable("TicketEvents");

        builder.HasKey(x => new { x.OrganizationId, x.TicketId, x.Id });

        //builder.OwnsOne(x => x.Event, x => x.ToJson());

        builder.HasIndex(x => x.TenantId);
    }
}
