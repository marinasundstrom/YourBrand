namespace YourBrand.Catalog.Domain.Entities;

public class ProductOption : Entity<int>
{
    public long ProductId { get; set; }

    public Product Product { get; set; } = null!;

    public string OptionId { get; set; } = null!;

    public Option Option { get; set; } = null!;

    //public bool IsSelected { get; set; }

    //public bool IsRequired { get; set; }

    public bool? IsInherited { get; set; }

    // Add fields for default values
}