using YourBrand.Payments.Domain.Common;

namespace YourBrand.Payments.Domain.Events;

public class PaymentCaptured : DomainEvent
{
    public PaymentCaptured(string paymentId)
    {
        PaymentId = paymentId;
    }

    public string PaymentId { get; }
}