using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace YourBrand.Catalog.Persistence.Configurations;

public class LanguageConfiguration : IEntityTypeConfiguration<Domain.Entities.Language>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Language> builder)
    {
        builder.ToTable("Languages");

        builder.HasKey(x => x.Code);
    }
}
