using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace YourBrand.Ticketing.Infrastructure.Persistence.Configurations;

public sealed class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.ToTable("Tickets");

        builder.HasKey(x => new { x.OrganizationId, x.Id });

        builder.HasIndex(x => x.TenantId);

        builder
            .HasOne(x => x.Type)
            .WithMany()
            .HasForeignKey(x => new { x.OrganizationId, x.TypeId })
            .OnDelete(DeleteBehavior.NoAction);

        builder
           .HasOne(x => x.Status)
           .WithMany()
           .HasForeignKey(x => new { x.OrganizationId, x.StatusId })
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasMany(x => x.Comments)
            .WithOne()
            .HasForeignKey(x => new { x.OrganizationId, x.TicketId })
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(x => x.Attachments)
            .WithOne()
            .HasForeignKey(x => new { x.OrganizationId, x.TicketId })
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(x => x.Tags)
            .WithOne()
            .HasForeignKey(x => new { x.OrganizationId, x.TicketId })
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.CreatedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.LastModifiedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

        builder.Ignore(e => e.DomainEvents);
    }
}