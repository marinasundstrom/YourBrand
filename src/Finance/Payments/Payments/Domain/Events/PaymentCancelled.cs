using YourBrand.Domain;
using YourBrand.Payments.Domain.Common;

namespace YourBrand.Payments.Domain.Events;

public record PaymentCancelled : DomainEvent
{
    public PaymentCancelled(string paymentId)
    {
        PaymentId = paymentId;
    }

    public string PaymentId { get; }
}