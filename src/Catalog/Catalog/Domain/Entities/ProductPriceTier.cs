using System;

using YourBrand.Auditability;
using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Tenancy;

namespace YourBrand.Catalog.Domain.Entities;

public class ProductPriceTier : Entity<string>, IHasTenant, IHasOrganization, IAuditableEntity<string>, ISoftDeletableWithAudit
{
    private ProductPriceTier()
    {
    }

    public ProductPriceTier(
        string productPriceId,
        int fromQuantity,
        int? toQuantity,
        ProductPriceTierType tierType,
        decimal value) : base(Guid.NewGuid().ToString())
    {
        ProductPriceId = productPriceId ?? throw new ArgumentNullException(nameof(productPriceId));

        Update(fromQuantity, toQuantity, tierType, value);
    }

    public TenantId TenantId { get; set; }

    public OrganizationId OrganizationId { get; set; }

    public string ProductPriceId { get; private set; } = default!;

    public ProductPrice ProductPrice { get; private set; } = default!;

    public int FromQuantity { get; private set; }

    public int? ToQuantity { get; private set; }

    public ProductPriceTierType TierType { get; private set; }

    public decimal Value { get; private set; }

    public void Update(int fromQuantity, int? toQuantity, ProductPriceTierType tierType, decimal value)
    {
        if (fromQuantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(fromQuantity), "Quantity must be greater than zero.");
        }

        if (toQuantity is not null && toQuantity < fromQuantity)
        {
            throw new ArgumentException("The upper quantity bound must be greater than or equal to the lower bound.", nameof(toQuantity));
        }

        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Value cannot be negative.");
        }

        FromQuantity = fromQuantity;
        ToQuantity = toQuantity;
        TierType = tierType;
        Value = value;
    }

    public UserId? CreatedById { get; set; }

    public DateTimeOffset Created { get; set; }

    public UserId? LastModifiedById { get; set; }

    public DateTimeOffset? LastModified { get; set; }

    public bool IsDeleted { get; set; }

    public DateTimeOffset? Deleted { get; set; }

    public UserId? DeletedById { get; set; }
}

public enum ProductPriceTierType
{
    PricePerUnit = 1,
    DiscountPerUnit = 2,
    DiscountPerAdditionalUnit = 3
}
