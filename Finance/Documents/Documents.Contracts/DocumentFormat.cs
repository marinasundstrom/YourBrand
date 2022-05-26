namespace Documents.Contracts;

[Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
public enum DocumentFormat
{
    Html = 1,
    Pdf = 2
}