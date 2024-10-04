using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace YourBrand.Agendas.Infrastructure.Persistence.Configurations;

public sealed class DebateConfiguration : IEntityTypeConfiguration<Debate>
{
    public void Configure(EntityTypeBuilder<Debate> builder)
    {
        builder.ToTable("Debate");

        builder.HasKey(x => new { x.OrganizationId, x.Id });

        builder.HasIndex(x => x.TenantId);

        builder.HasMany(x => x.Entries)
            .WithOne()
            .HasForeignKey(x => new { x.OrganizationId, x.DebateId });
            
        builder.Navigation(x => x.Entries).AutoInclude();

        builder.HasOne(x => x.CreatedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.LastModifiedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

        builder.Ignore(e => e.DomainEvents);
    }
}
