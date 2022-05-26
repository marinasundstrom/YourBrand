namespace Invoices.Domain.Enums;

[Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
public enum InvoiceStatus
{
    Draft,
    Sent,
    Paid,
    PartiallyPaid,
    Overpaid,
    Repaid,
    PartiallyRepaid,
    Reminder,
    Void
}
