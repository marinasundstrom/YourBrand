namespace YourBrand.Payments.Domain.Enums;

[Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
public enum PaymentStatus
{
    Unverified,
    Verified,
    Payback,
    Unknown,
}
