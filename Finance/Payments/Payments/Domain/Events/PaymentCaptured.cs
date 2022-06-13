using YourBrand.Payments.Domain.Common;

namespace YourBrand.Payments.Domain.Events;

public class PaymentCaptured : DomainEvent
{
    public PaymentCaptured(string paymentId, string captureId)
    {
        PaymentId = paymentId;
        CaptureId = captureId;
    }

    public string PaymentId { get; }

    public string CaptureId { get; }
}
