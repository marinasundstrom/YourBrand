namespace YourBrand.Payments.Domain.Enums;

[Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
public enum PaymentStatus
{
    Created,
    Captured,
    PartiallyCaptured,
    Refunded,
    PartiallyRefunded,
    Cancelled
}