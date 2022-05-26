namespace Documents.Domain.Enums;

[Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
public enum DocumentTemplateLanguage
{
    Razor = 1
}