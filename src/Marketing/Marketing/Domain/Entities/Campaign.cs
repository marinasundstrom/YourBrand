using YourBrand.Identity;

namespace YourBrand.Marketing.Domain.Entities;

public class Campaign : Entity<string>, IAuditable
{
    readonly HashSet<ProductOffer> _productOffers = new HashSet<ProductOffer>();

#nullable disable

    protected Campaign() : base() { }

#nullable restore

    public Campaign(string name)
    : base(Guid.NewGuid().ToString())
    {
        Name = name;
    }

    public string Name { get; set; }

    public string? Description { get; set; }

    public DateTimeOffset StartDate { get; set; }

    public DateTimeOffset EndTime { get; set; }

    public IReadOnlyCollection<ProductOffer> ProductOffers => _productOffers;

    public void AddProductOffer(ProductOffer productOffer) => _productOffers.Add(productOffer);

    public bool RemoveProductOffer(ProductOffer productOffer) => _productOffers.Remove(productOffer);

    public UserId? CreatedById { get; set; } = null!;

    public DateTimeOffset Created { get; set; }

    public UserId? LastModifiedById { get; set; }

    public DateTimeOffset? LastModified { get; set; }
}