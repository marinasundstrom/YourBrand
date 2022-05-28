using YourBrand.Payments.Domain.Common;

namespace YourBrand.Payments.Domain.Events;

public class PaymentRegistered : DomainEvent
{
    public PaymentRegistered(string paymentId)
    {
        PaymentId = paymentId;
    }

    public string PaymentId { get; }
}