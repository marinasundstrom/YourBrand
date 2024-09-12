using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace YourBrand.Ticketing.Infrastructure.Persistence.Configurations;

public sealed class TicketCategoryConfiguration : IEntityTypeConfiguration<TicketCategory>
{
    public void Configure(EntityTypeBuilder<TicketCategory> builder)
    {
        builder.ToTable("TicketCategories");

        builder.HasKey(x => new { x.OrganizationId, x.Id });

        builder.HasIndex(x => x.TenantId);

        builder
            .HasOne(x => x.TicketType)
            .WithMany()
            .HasForeignKey(x => new { x.OrganizationId, x.TicketTypeId })
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasOne(x => x.Parent)
            .WithMany(x => x.SubCategories)
            .HasForeignKey(x => new { x.OrganizationId, x.ParentId })
            .OnDelete(DeleteBehavior.ClientCascade);
    }
}