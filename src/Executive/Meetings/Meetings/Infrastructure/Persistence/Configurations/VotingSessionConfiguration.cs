using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace YourBrand.Agendas.Infrastructure.Persistence.Configurations;

public sealed class VotingConfiguration : IEntityTypeConfiguration<Voting>
{
    public void Configure(EntityTypeBuilder<Voting> builder)
    {
        builder.ToTable("Voting");

        builder.HasKey(x => new { x.OrganizationId, x.Id });

        builder.HasIndex(x => x.TenantId);

        builder.HasMany(x => x.Votes)
            .WithOne()
            .HasForeignKey(x => new { x.OrganizationId, x.VotingId });

        builder.Navigation(x => x.Votes).AutoInclude();

        builder.HasOne(x => x.CreatedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.LastModifiedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

        builder.Ignore(e => e.DomainEvents);
    }
}
