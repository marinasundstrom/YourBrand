namespace YourBrand.Payments.Contracts;

public record PaymentBatch(string OrganizationId, IEnumerable<Payment> Payments);

public record Payment(string OrganizationId, string? Id, int InvoiceId, PaymentStatus Status, string Currency, decimal Amount, DateTime DueDate, PaymentMethod PaymentMethod, string? Message = null);

public enum PaymentStatus
{
    Created,
    Captured,
    PartiallyCaptured,
    Refunded,
    PartiallyRefunded,
    Cancelled
}

public enum PaymentMethod
{
    PlusGiro
}

public record PaymentStatusChanged(string OrganizationId, string PaymentId, PaymentStatus Status);

public record PaymentCaptured(string OrganizationId, string PaymentId, string CaptureId, DateTime Date, string Currency, decimal Amount);