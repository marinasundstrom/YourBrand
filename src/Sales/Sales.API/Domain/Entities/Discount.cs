using YourBrand.Auditability;
using YourBrand.Domain;

namespace YourBrand.Sales.Domain.Entities;

public class Discount : Entity<string>, IAuditableEntity<string, User>, IHasTenant
{
    public Discount() : base(Guid.NewGuid().ToString())
    {

    }

    public TenantId TenantId { get; set; }

    public OrganizationId OrganizationId { get; set; }

    public string? OrderId { get; set; }

    public string? OrderItemId { get; set; }

    public string Description { get; set; }

    public double? Rate { get; set; }

    public decimal? Amount { get; set; }

    public decimal? Total { get; private set; }

    public DateTimeOffset? EffectiveDate { get; private set; }

    public DateTimeOffset? ExpiryDate { get; private set; }

    public decimal ApplyTo(Order order)
    {
        return (Total = CalculateTotal(order)).GetValueOrDefault();
    }

    public decimal ApplyTo(OrderItem orderItem)
    {
        return (Total = CalculateTotal(orderItem)).GetValueOrDefault();
    }

    public decimal CalculateTotal(Order order)
    {
        if (Rate.HasValue)
            return order.SubTotal * (decimal)Rate;
        return Amount ?? 0;
    }

    public decimal CalculateTotal(OrderItem orderItem)
    {
        if (Rate.HasValue)
            return orderItem.Price * (decimal)orderItem.Quantity * (decimal)Rate.Value;
        return Amount ?? 0;
    }
    public bool IsValid(TimeProvider timeProvider)
            => (EffectiveDate == null || timeProvider.GetUtcNow() >= EffectiveDate)
               && (ExpiryDate == null || timeProvider.GetUtcNow() <= ExpiryDate);

    // External code
    public string? DiscountId { get; set; }

    public User? CreatedBy { get; set; }

    public UserId? CreatedById { get; set; }

    public DateTimeOffset Created { get; set; }

    public User? LastModifiedBy { get; set; }

    public UserId? LastModifiedById { get; set; }

    public DateTimeOffset? LastModified { get; set; }
}