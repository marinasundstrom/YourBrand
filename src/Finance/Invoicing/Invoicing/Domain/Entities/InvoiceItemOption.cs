using YourBrand.Auditability;
using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Invoicing.Domain.Common;
using YourBrand.Tenancy;

namespace YourBrand.Invoicing.Domain.Entities;

public class InvoiceItemOption : AuditableEntity<string>, IHasTenant, IHasOrganization
{
    public InvoiceItemOption(string name, string? description, string? value, string? productId, string? itemId, decimal? price, decimal? discount) : base(Guid.NewGuid().ToString())
    {
        Name = name;
        Description = description;
        Value = value;
        ProductId = productId;
        ItemId = itemId;
        Price = price;
        Discount = discount;
    }

    internal InvoiceItemOption() : base(Guid.NewGuid().ToString())
    {

    }

    public TenantId TenantId { get; set; }

    public OrganizationId OrganizationId { get; set; }

    public string? InvoiceId { get; set; }

    public string? InvoiceItemId { get; set; }

    public string Name { get; set;}

    public string? Description { get; set; }

    public string? Value { get; set; }

    public string? ProductId { get; set; }

    public string? ItemId { get; set; }

    public decimal? Price { get; set; }

    public decimal? Discount { get; set; }
}