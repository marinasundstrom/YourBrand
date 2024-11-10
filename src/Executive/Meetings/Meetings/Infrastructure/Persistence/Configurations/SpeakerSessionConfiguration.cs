using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace YourBrand.Agendas.Infrastructure.Persistence.Configurations;

public sealed class SpeakerSessionConfiguration : IEntityTypeConfiguration<SpeakerSession>
{
    public void Configure(EntityTypeBuilder<SpeakerSession> builder)
    {
        builder.ToTable("SpeakerSessions");

        builder.HasKey(x => new { x.OrganizationId, x.Id });

        builder.HasIndex(x => x.TenantId);

        builder.HasMany(x => x.SpeakerQueue)
            .WithOne()
            .HasForeignKey(x => new { x.OrganizationId, x.SpeakerSessionId });

        /*
        builder
            .Property(x => x.SpeakerQueue)
            .HasField("_speakerQueue"); */

        builder
            .Navigation(x => x.SpeakerQueue).AutoInclude();

        builder.HasOne(x => x.CurrentSpeaker)
            .WithMany()
            .HasForeignKey(x => new { x.OrganizationId, x.CurrentSpeakerId });

        builder.Navigation(x => x.CurrentSpeaker).AutoInclude();

        builder.HasOne(x => x.CreatedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.LastModifiedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

        builder.Ignore(e => e.DomainEvents);
    }
}
