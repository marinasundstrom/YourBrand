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
            .HasOne(x => x.Category)
            .WithMany()
            .HasForeignKey(x => new { x.OrganizationId, x.CategoryId })
            .OnDelete(DeleteBehavior.NoAction);

        builder
           .HasOne(x => x.Status)
           .WithMany()
           .HasForeignKey(x => new { x.OrganizationId, x.StatusId })
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.ReportedBy)
            .WithMany()
            .HasForeignKey(x => new { x.OrganizationId, x.Id, x.ReportedById })
            .OnDelete(DeleteBehavior.ClientCascade);

        builder.HasOne(x => x.Assignee)
            .WithMany()
            .HasForeignKey(x => new { x.OrganizationId, x.Id, x.AssigneeId })
            .OnDelete(DeleteBehavior.ClientCascade);

        builder
            .HasMany(x => x.Participants)
            .WithOne()
            .HasForeignKey(x => new { x.OrganizationId, x.TicketId })
            .OnDelete(DeleteBehavior.ClientCascade)
            .IsRequired(false);

        builder
            .HasMany(x => x.Comments)
            .WithOne()
            .HasForeignKey(x => new { x.OrganizationId, x.TicketId })
            .OnDelete(DeleteBehavior.ClientCascade);

        builder
            .HasMany(x => x.Attachments)
            .WithOne()
            .HasForeignKey(x => new { x.OrganizationId, x.TicketId })
            .OnDelete(DeleteBehavior.ClientCascade);

        builder
            .HasMany(x => x.Tags)
            .WithOne()
            .HasForeignKey(x => new { x.OrganizationId, x.TicketId })
            .OnDelete(DeleteBehavior.ClientCascade);

        builder
            .HasMany(x => x.Events)
            .WithOne()
            .HasForeignKey(x => new { x.OrganizationId, x.TicketId })
            .OnDelete(DeleteBehavior.ClientCascade);

        builder.HasOne(x => x.CreatedBy)
            .WithMany()
            .HasForeignKey(x => new { x.OrganizationId, x.Id, x.CreatedById })
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.LastModifiedBy)
            .WithMany()
            .HasForeignKey(x => new { x.OrganizationId, x.Id, x.LastModifiedById })
            .OnDelete(DeleteBehavior.NoAction);

        builder.Ignore(e => e.DomainEvents);
    }
}