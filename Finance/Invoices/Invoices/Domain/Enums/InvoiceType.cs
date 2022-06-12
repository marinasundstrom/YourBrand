namespace YourBrand.Invoices.Domain.Enums;

[Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
public enum InvoiceType 
{
    Invoice = 1,
    Credit = 2
}