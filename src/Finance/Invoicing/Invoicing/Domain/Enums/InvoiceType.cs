namespace YourBrand.Invoicing.Domain.Enums;

[Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
public enum InvoiceType
{
    Invoice = 0,
    Cash = 1,
    Credit = 2
}