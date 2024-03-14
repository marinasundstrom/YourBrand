namespace YourBrand.Catalog.Domain.Entities;

public class ProductImage
{
    public ProductImage() { }

    public ProductImage(string title, string text, string url)
    {
        Id = Guid.NewGuid().ToString();
        Title = title;
        Text = text;
        Url = url;
    }

    public string Id { get; private set; }

    public Store? Store { get; set; }

    public string? StoreId { get; set; }

    public Product? Product { get; set; } = default!;

    public long? ProductId { get; set; }

    public string Title { get; set; }

    public string? Text { get; set; }

    public string Url { get; set; }
}