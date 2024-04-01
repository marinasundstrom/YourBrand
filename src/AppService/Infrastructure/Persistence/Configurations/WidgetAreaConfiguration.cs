using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Domain.Entities;

namespace YourBrand.Infrastructure.Persistence.Configurations;

public sealed class WidgetAreaConfiguration : IEntityTypeConfiguration<WidgetArea>
{
    public void Configure(EntityTypeBuilder<WidgetArea> builder)
    {
        builder.ToTable("WidgetAreas");
    }
}