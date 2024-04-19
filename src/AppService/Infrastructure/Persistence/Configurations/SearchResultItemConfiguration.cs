using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Domain.Entities;

namespace YourBrand.Infrastructure.Persistence.Configurations;

sealed class SearchResultItemConfiguration : IEntityTypeConfiguration<SearchResultItem>
{
    public void Configure(EntityTypeBuilder<SearchResultItem> builder)
    {
        builder.ToTable("SearchResultItems");
        builder.HasQueryFilter(i => i.Deleted == null);

        builder.HasIndex(nameof(SearchResultItem.Name), nameof(SearchResultItem.Description));

        builder.Ignore(i => i.DomainEvents);
    }
}