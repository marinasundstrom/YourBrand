
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace YourBrand.TimeReport.Features.Common.Models;

[JsonConverter(typeof(StringEnumConverter))]
public enum SortDirection
{
    Asc = 2,
    Desc = 1
}