namespace YourBrand.Invoicing.Domain.Enums;

[Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
public enum InvoiceType 
{
    Invoice = 1,
    Cash = 2,
    Credit = 3
}