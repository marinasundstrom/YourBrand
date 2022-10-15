using YourBrand.Marketing.Domain.Common;

namespace YourBrand.Marketing.Domain.Entities;

public class Discount : AuditableEntity
{
    protected Discount()
    {

    }

    public Discount(string productId, string productName, string? productDescription, decimal ordinaryPrice, double percent)
    {
        Id = Guid.NewGuid().ToString();
        ProductId = productId;
        ProductName = productName;
        ProductDescription1 = productDescription;
        OrdinaryPrice = ordinaryPrice;
        Percent = percent;
    }

    public string Id { get; private set; }

    public Contact? Contact { get; set; }

    public string ProductId { get; set; } = null!;
    public string ProductName { get; set; }
    public string? ProductDescription1 { get; }
    public decimal? ProductDescription { get; set; }

    public double Percent { get; set; }
    public decimal OrdinaryPrice { get; set; }
    public decimal DiscountedPrice { get; set; }
}