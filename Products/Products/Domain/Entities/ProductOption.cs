namespace YourBrand.Products.Domain.Entities;

public class ProductOption
{
    public int Id { get; set; }

    public string ProductId { get; set; } = null!;

    public Product Product { get; set; } = null!;

    public string OptionId { get; set; } = null!;

    public Option Option { get; set; } = null!;

    public bool? IsSelected { get; set; }
}
