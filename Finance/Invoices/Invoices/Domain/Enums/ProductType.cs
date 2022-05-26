namespace Invoices.Domain.Enums;

[Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
public enum ProductType 
{
    Good = 1,
    Service = 2
}