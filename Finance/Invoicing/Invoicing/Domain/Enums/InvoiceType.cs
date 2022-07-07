namespace YourBrand.Invoicing.Domain.Enums;

[Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
public enum InvoiceType 
{
    Invoice = 0,
    Credit = 1
}