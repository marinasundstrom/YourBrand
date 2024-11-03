using YourBrand.Domain;
using YourBrand.Payments.Domain.Common;
using YourBrand.Payments.Domain.Enums;

namespace YourBrand.Payments.Domain.Events;

public record PaymentStatusChanged : DomainEvent
{
    public PaymentStatusChanged(string paymentId, PaymentStatus status)
    {
        PaymentId = paymentId;
        Status = status;
    }

    public string PaymentId { get; }

    public PaymentStatus Status { get; }
}