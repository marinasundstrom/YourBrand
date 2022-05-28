using YourBrand.Payments.Domain.Common;

namespace YourBrand.Payments.Domain.Events;

public class PaymentCreated : DomainEvent
{
    public PaymentCreated(string paymentId)
    {
        PaymentId = paymentId;
    }

    public string PaymentId { get; }
}
