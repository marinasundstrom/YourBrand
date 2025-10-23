using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace YourBrand.Agendas.Infrastructure.Persistence.Configurations;

public sealed class ElectionConfiguration : IEntityTypeConfiguration<Election>
{
    public void Configure(EntityTypeBuilder<Election> builder)
    {
        builder.ToTable("Elections");

        builder.HasKey(x => new { x.OrganizationId, x.Id });

        builder.HasIndex(x => x.TenantId);

        builder.HasMany(x => x.Candidates)
            .WithOne()
            .HasForeignKey(x => new { x.OrganizationId, x.ElectionId });

        builder.Navigation(x => x.Candidates).AutoInclude();

        builder.HasMany(x => x.Ballots)
            .WithOne()
            .HasForeignKey(x => new { x.OrganizationId, x.ElectionId });

        builder.Navigation(x => x.Ballots).AutoInclude();

        builder.HasOne(x => x.MeetingFunction)
            .WithMany()
            .HasForeignKey(x => x.MeetingFunctionId);

        builder.Navigation(x => x.MeetingFunction).AutoInclude();

        builder.HasOne(x => x.ElectedCandidate)
            .WithMany()
            .HasForeignKey(x => new { x.OrganizationId, x.ElectedCandidateId });

        builder.Navigation(x => x.ElectedCandidate).AutoInclude();

        builder.HasOne(x => x.CreatedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.LastModifiedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

        builder.Ignore(e => e.DomainEvents);
    }
}