using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace YourBrand.Accounting.Application.Entries;

[JsonConverter(typeof(StringEnumConverter))]
public enum ResultDirection
{
    Desc,
    Asc
}