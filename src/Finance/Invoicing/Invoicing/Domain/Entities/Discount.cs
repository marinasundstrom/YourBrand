using YourBrand.Domain;
using YourBrand.Invoicing.Domain.Common;
using YourBrand.Tenancy;

namespace YourBrand.Invoicing.Domain.Entities;

public class Discount : AuditableEntity<string>, IHasTenant, IHasOrganization
{
    public Discount() : base(Guid.NewGuid().ToString())
    {

    }

    public TenantId TenantId { get; set; }

    public OrganizationId OrganizationId { get; set; }

    public string? InvoiceId { get; set; }

    public string? InvoiceItemId { get; set; }

    public string Description { get; set; }

    public double? Rate { get; set; }

    public decimal? Amount { get; set; }

    public decimal? Total { get; private set; }

    public DateTimeOffset? EffectiveDate { get; private set; }

    public DateTimeOffset? ExpiryDate { get; private set; }

    public decimal ApplyTo(Invoice invoice)
    {
        return (Total = CalculateTotal(invoice)).GetValueOrDefault();
    }

    public decimal ApplyTo(InvoiceItem invoiceItem)
    {
        return (Total = CalculateTotal(invoiceItem)).GetValueOrDefault();
    }

    public decimal CalculateTotal(Invoice invoice)
    {
        if (Rate.HasValue)
            return invoice.SubTotal * (decimal)Rate;
        return Amount ?? 0;
    }

    public decimal CalculateTotal(InvoiceItem invoiceItem)
    {
        if (Rate.HasValue)
            return invoiceItem.Price * (decimal)invoiceItem.Quantity * (decimal)Rate.Value;
        return Amount ?? 0;
    }
    public bool IsValid(TimeProvider timeProvider)
            => (EffectiveDate == null || timeProvider.GetUtcNow() >= EffectiveDate)
               && (ExpiryDate == null || timeProvider.GetUtcNow() <= ExpiryDate);

    // External code
    public string? DiscountId { get; set; }
}