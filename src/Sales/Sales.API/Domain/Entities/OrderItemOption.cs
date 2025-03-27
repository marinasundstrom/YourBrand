using YourBrand.Auditability;
using YourBrand.Domain;

namespace YourBrand.Sales.Domain.Entities;

public class OrderItemOption : Entity<string>, IAuditableEntity<string, User>, IHasTenant
{
    public OrderItemOption(string name, string? description, string? value, string? productId, string? itemId, decimal? price, decimal? discount) : base(Guid.NewGuid().ToString())
    {
        Name = name;
        Description = description;
        Value = value; 
        ProductId = productId;
        ItemId = itemId;
        Price = price;
        Discount = discount;
    }

    internal OrderItemOption() : base(Guid.NewGuid().ToString())
    {

    }

    public TenantId TenantId { get; set; }

    public OrganizationId OrganizationId { get; set; }

    public string? OrderId { get; set; }

    public string? OrderItemId { get; set; }

    public string Name { get; set; }

    public string? Description { get; set; }

    public string? Value { get; set; }

    public string? ProductId { get; set; }

    public string? ItemId { get; set; }

    public decimal? Price { get; set; }

    public decimal? Discount { get; set; }

    public User? CreatedBy { get; set; }

    public UserId? CreatedById { get; set; }

    public DateTimeOffset Created { get; set; }

    public User? LastModifiedBy { get; set; }

    public UserId? LastModifiedById { get; set; }

    public DateTimeOffset? LastModified { get; set; }
}