namespace YourBrand.Payments.Contracts;

public record PaymentBatch(IEnumerable<Payment> Payments);

public record Payment(string Id, DateTime Date, PaymentStatus Status, string From, string Reference, string Currency, decimal Amount);

public enum PaymentStatus
{
    Unverified,
    Verified,
    Payback,
    Unknown,
}