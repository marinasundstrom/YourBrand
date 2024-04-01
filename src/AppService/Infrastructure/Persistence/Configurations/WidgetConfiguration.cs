using System.Text.Json;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Domain.Entities;

namespace YourBrand.Infrastructure.Persistence.Configurations;

public sealed class WidgetConfiguration : IEntityTypeConfiguration<Widget>
{
    public void Configure(EntityTypeBuilder<Widget> builder)
    {
        builder.ToTable("Widgets");

        builder
            .Property(x => x.Settings)
            .HasConversion(x => x == null ? null : x.RootElement.ToString(), x => x == null ? null : JsonDocument.Parse(x, new JsonDocumentOptions()));
    }
}
