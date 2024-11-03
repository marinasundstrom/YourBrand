using YourBrand.Domain;
using YourBrand.Payments.Domain.Common;

namespace YourBrand.Payments.Domain.Events;

public record PaymentCreated : DomainEvent
{
    public PaymentCreated(string paymentId)
    {
        PaymentId = paymentId;
    }

    public string PaymentId { get; }
}