namespace YourBrand.Marketing.Domain.Entities;

public class ProductOffer : Entity<string>, IAuditable
{
#nullable disable

    protected ProductOffer() : base() { }

#nullable restore

    public ProductOffer(string name)
    : base(Guid.NewGuid().ToString())
    {
        Name = name;
    }

    public string CampaignId { get; private set; } = null!;

    public string ProductId { get; set; } = null!;

    public string? VariantId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal OfferPrice { get; set; }

    public string? CreatedById { get; set; } = null!;

    public DateTimeOffset Created { get; set; }

    public string? LastModifiedById { get; set; }

    public DateTimeOffset? LastModified { get; set; }
}
