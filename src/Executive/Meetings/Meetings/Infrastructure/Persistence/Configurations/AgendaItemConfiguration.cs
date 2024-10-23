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

        builder.HasOne(x => x.SpeakerSession)
            .WithOne()
            .HasForeignKey<SpeakerSession>(x => new { x.OrganizationId, x.AgendaItemId });

        builder.Navigation(x => x.SpeakerSession).AutoInclude();

        builder.HasOne(x => x.VotingSession)
            .WithOne()
            .HasForeignKey<VotingSession>(x => new { x.OrganizationId, x.AgendaItemId });

        builder.Navigation(x => x.VotingSession).AutoInclude();

        builder.HasMany(x => x.Candidates)
            .WithOne()
            .HasForeignKey(x => new { x.OrganizationId, x.AgendaItemId });

        builder.Navigation(x => x.Candidates).AutoInclude();

        builder.HasOne(x => x.CreatedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.LastModifiedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

        builder.Ignore(e => e.DomainEvents);
    }
}