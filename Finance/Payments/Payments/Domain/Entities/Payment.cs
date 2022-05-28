using YourBrand.Payments.Domain.Common;
using YourBrand.Payments.Domain.Enums;
using YourBrand.Payments.Domain.Events;

namespace YourBrand.Payments.Domain.Entities;

public class Payment : AuditableEntity, IHasDomainEvents
{
    readonly List<Capture> _captures = new List<Capture>();

    private Payment()
    {

    }

    public Payment(int invoiceId, PaymentStatus status, string currency, decimal amount, DateTime dueDate, PaymentMethod paymentMethod, string? reference = null, string? message = null)
    {
        if(amount <= 0)
        {
            throw new ArgumentException("Amount must be greater than 0.");
        }

        Id = Guid.NewGuid().ToUrlFriendlyString();
        InvoiceId = invoiceId;
        Status = status;
        DueDate = dueDate;
        Currency = currency;
        Amount = amount;
        PaymentMethod = paymentMethod;
        Reference = reference;
        Message = message;

        DomainEvents.Add(new PaymentCreated(Id));
    }

    public void SetStatus(PaymentStatus status)
    {
        if(Status != status)
        {
            Status = status;

            DomainEvents.Add(new PaymentStatusChanged(Id, status));
        }
    }

    public string Id { get; private set; } = null!;

    public int InvoiceId { get; private set; }

    public PaymentStatus Status { get; private set; } = PaymentStatus.Created;

    public DateTime DueDate { get; private set; }

    public string Currency { get; private set; } = null!;

    public decimal Amount { get; private set; }

    public PaymentMethod PaymentMethod { get; private set; }

    public string? Reference { get; private set; }

    public string? Message { get; private set; }

    public IReadOnlyCollection<Capture> Captures => _captures.AsReadOnly();

    public void RegisterCapture(DateTime date, decimal amount, string? transactionId) 
    {
        _captures.Add(new Capture(date, amount, transactionId));
    }

    public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
}
