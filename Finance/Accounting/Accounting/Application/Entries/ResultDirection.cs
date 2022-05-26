using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Accounting.Application.Entries;

[JsonConverter(typeof(StringEnumConverter))]
public enum ResultDirection
{
    Desc,
    Asc
}