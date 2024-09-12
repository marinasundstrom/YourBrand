using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Ticketing.Domain.Entities;

namespace YourBrand.Ticketing.Infrastructure.Persistence.Configurations;

public class TicketTagConfiguration : IEntityTypeConfiguration<TicketTag>
{
    public void Configure(EntityTypeBuilder<TicketTag> builder)
    {
        builder.ToTable(name: "TicketTags");

        builder.HasIndex(x => x.TenantId);

        builder
            .HasOne(x => x.Organization)
            .WithMany()
            .HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasOne(x => x.Ticket)
            .WithMany()
            .HasForeignKey(x => new { x.OrganizationId, x.TicketId })
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasOne(x => x.Tag)
            .WithMany()
            .HasForeignKey(x => new { x.OrganizationId, x.TagId })
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.CreatedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.LastModifiedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

        builder.Ignore(e => e.DomainEvents);
    }
}