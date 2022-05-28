using YourBrand.Payments.Domain.Common;
using YourBrand.Payments.Domain.Enums;
using YourBrand.Payments.Domain.Events;

namespace YourBrand.Payments.Domain.Entities;

public class Payment : AuditableEntity, IHasDomainEvents
{
    private Payment()
    {

    }

    public Payment(string? id, int invoiceId, PaymentStatus status, string currency, decimal amount, DateTime dueDate, PaymentMethod paymentMethod)
    {
        if(amount <= 0)
        {
            throw new ArgumentException("Amount must be greater than 0.");
        }

        Id = id ?? Guid.NewGuid().ToUrlFriendlyString();
        InvoiceId = invoiceId;
        Status = status;
        DueDate = dueDate;
        Currency = currency;
        Amount = amount;
        PaymentMethod = paymentMethod;

        DomainEvents.Add(new PaymentRegistered(Id));
    }

    public void SetStatus(PaymentStatus status)
    {
        if(Status != status)
        {
            Status = status;

            DomainEvents.Add(new PaymentStatusChanged(Id, status));
        }
    }

    public string Id { get; set; } = null!;

    public int InvoiceId { get; set; }

    public PaymentStatus Status { get; private set; } = PaymentStatus.Unverified;

    public DateTime DueDate { get; set; }

    public string Currency { get; set; } = null!;

    public decimal Amount { get; set; }

    public PaymentMethod PaymentMethod { get; set; }

    public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
}