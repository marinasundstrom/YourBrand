using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace YourBrand.Ticketing.Infrastructure.Persistence.Configurations;

public sealed class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("Projects");

        builder.HasKey(x => new { x.OrganizationId, x.Id });

        builder.HasIndex(x => x.TenantId);

        builder
            .HasMany<Ticket>()
            .WithOne()
            .HasForeignKey(x => new { x.OrganizationId, x.ProjectId })
            .OnDelete(DeleteBehavior.ClientCascade);

        builder
            .HasMany(x => x.TicketTypes)
            .WithOne()
            .HasForeignKey(x => new { x.OrganizationId, x.ProjectId })
            .OnDelete(DeleteBehavior.ClientCascade);

        builder
            .HasMany(x => x.TicketStatuses)
            .WithOne()
            .HasForeignKey(x => new { x.OrganizationId, x.ProjectId })
            .OnDelete(DeleteBehavior.ClientCascade);

        builder
            .HasMany(x => x.TicketCategories)
            .WithOne()
            .HasForeignKey(x => new { x.OrganizationId, x.ProjectId })
            .OnDelete(DeleteBehavior.ClientCascade);

        builder
            .HasMany(x => x.Tags)
            .WithOne()
            .HasForeignKey(x => new { x.OrganizationId, x.ProjectId })
            .OnDelete(DeleteBehavior.ClientCascade);

        builder.HasOne(x => x.CreatedBy)
            .WithMany()
            //.HasForeignKey(x => new { x.OrganizationId, x.CreatedById })
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.LastModifiedBy)
            .WithMany()
            //.HasForeignKey(x => new { x.OrganizationId, x.LastModifiedById })
            .OnDelete(DeleteBehavior.NoAction);

        builder.Ignore(e => e.DomainEvents);
    }
}