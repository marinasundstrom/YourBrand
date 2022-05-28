namespace YourBrand.Payments.Contracts;

public record PaymentBatch(IEnumerable<Payment> Payments);

public record Payment(string? Id, int InvoiceId, PaymentStatus Status, string Currency, decimal Amount, DateTime DueDate, PaymentMethod PaymentMethod, string? Message = null);

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

public record PaymentStatusChanged(string PaymentId, PaymentStatus Status);

public record PaymentCaptured(string PaymentId, string CaptureId, DateTime Date, string Currency, decimal Amount);