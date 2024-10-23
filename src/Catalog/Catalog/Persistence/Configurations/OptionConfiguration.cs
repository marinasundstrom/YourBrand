using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Catalog.Domain.Entities;

namespace YourBrand.Catalog.Persistence.Configurations;

public class OptionConfiguration : IEntityTypeConfiguration<Option>
{
    public void Configure(EntityTypeBuilder<Option> builder)
    {
        builder.ToTable("Options");

        builder.HasKey(x => new { x.OrganizationId, x.Id });

        builder.HasDiscriminator(x => x.OptionType)
            .HasValue(typeof(SelectableOption), Domain.Enums.OptionType.YesOrNo)
            .HasValue(typeof(ChoiceOption), Domain.Enums.OptionType.Choice)
            .HasValue(typeof(NumericalValueOption), Domain.Enums.OptionType.NumericalValue)
            .HasValue(typeof(TextValueOption), Domain.Enums.OptionType.TextValue);

        builder.HasOne(o => o.ProductCategory).WithMany(x => x.Options)
            .HasForeignKey(o => new { o.OrganizationId, o.ProductCategoryId })
            .OnDelete(DeleteBehavior.ClientNoAction);

        builder.HasOne(o => o.Group).WithMany(x => x.Options)
            .HasForeignKey(o => new { o.OrganizationId, o.GroupId })
            .OnDelete(DeleteBehavior.ClientNoAction);
    }
}


public class ChoiceOptionConfiguration : IEntityTypeConfiguration<ChoiceOption>
{
    public void Configure(EntityTypeBuilder<ChoiceOption> builder)
    {
        builder
            .HasMany(p => p.Values)
            .WithOne(p => p.Option);

        builder.HasOne(p => p.DefaultValue)
            .WithMany()
            .HasForeignKey(x => new { x.OrganizationId, x.DefaultValueId });

        builder.HasMany(p => p.Values)
            .WithOne(x => x.Option)
            .HasForeignKey(x => new { x.OrganizationId, x.OptionId });
    }
}

public class SelectableOptionConfiguration : IEntityTypeConfiguration<SelectableOption>
{
    public void Configure(EntityTypeBuilder<SelectableOption> builder)
    {
        builder.Property(x => x.Price).HasColumnName(nameof(SelectableOption.Price));
    }
}

public class NumericalValueOptionConfiguration : IEntityTypeConfiguration<NumericalValueOption>
{
    public void Configure(EntityTypeBuilder<NumericalValueOption> builder)
    {
        builder.Property(x => x.Price).HasColumnName(nameof(NumericalValueOption.Price));
    }
}