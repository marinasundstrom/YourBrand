using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace YourBrand.ApiKeys.Infrastructure.Persistence.Configurations;

sealed class ApplicationConfiguration : IEntityTypeConfiguration<Domain.Entities.Application>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Application> builder)
    {
        builder.ToTable("Applications");
        builder.HasQueryFilter(i => i.Deleted == null);

        builder.Ignore(i => i.DomainEvents);
    }
}