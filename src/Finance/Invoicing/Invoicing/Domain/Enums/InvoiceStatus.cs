namespace YourBrand.Invoicing.Domain.Enums;

[Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
public enum InvoiceStatus
{
    Draft = 1,
    Sent,
    Paid,
    PartiallyPaid,
    Overpaid,
    Repaid,
    PartiallyRepaid,
    Reminder,
    Void
}