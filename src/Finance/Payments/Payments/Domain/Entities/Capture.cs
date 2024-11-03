using YourBrand.Domain;
using YourBrand.Payments.Domain.Common;
using YourBrand.Tenancy;

namespace YourBrand.Payments.Domain.Entities;

public class Capture : Entity<string>, IHasTenant, IHasOrganization
{
    private Capture()
    {

    }

    public Capture(DateTime date, decimal amount, string? transactionId)
     : base(Guid.NewGuid().ToString())
    {
        Date = date;
        Amount = amount;
        TransactionId = transactionId;
    }

    public TenantId TenantId { get; set; }

    public OrganizationId OrganizationId { get; set; }

    public string PaymentId { get; set; } = null!;

    public DateTime Date { get; set; }

    public decimal Amount { get; set; }

    public string? TransactionId { get; set; }
}