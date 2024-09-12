using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace YourBrand.Ticketing.Infrastructure.Persistence.Configurations;

public sealed class TicketParticipantConfiguration : IEntityTypeConfiguration<TicketParticipant>
{
    public void Configure(EntityTypeBuilder<TicketParticipant> builder)
    {
        builder.ToTable("TicketParticipants");

        builder.HasKey(x => new { x.OrganizationId, x.TicketId, x.Id });

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