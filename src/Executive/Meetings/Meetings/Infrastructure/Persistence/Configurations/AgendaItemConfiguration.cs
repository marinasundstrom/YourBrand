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

        builder.HasOne(x => x.Type)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

        builder.Navigation(x => x.Type).AutoInclude();

        builder.HasMany(x => x.SubItems)
            .WithOne()
            .HasForeignKey(x => new { x.OrganizationId, x.ParentId });

       //builder.Navigation(x => x.SubItems).AutoInclude();

        builder.HasOne(x => x.Discussion)
            .WithOne()
            .HasForeignKey<Discussion>(x => new { x.OrganizationId, x.AgendaItemId });

        builder.Navigation(x => x.Discussion).AutoInclude();

        builder.HasOne(x => x.Voting)
            .WithOne()
            .HasForeignKey<Voting>(x => new { x.OrganizationId, x.AgendaItemId });

        builder.Navigation(x => x.Voting).AutoInclude();

        builder.HasOne(x => x.Election)
            .WithOne()
            .HasForeignKey<Election>(x => new { x.OrganizationId, x.AgendaItemId });

        builder.Navigation(x => x.Election).AutoInclude();

        builder.HasOne(x => x.CreatedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.LastModifiedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

        builder.Ignore(e => e.DomainEvents);
    }
}
