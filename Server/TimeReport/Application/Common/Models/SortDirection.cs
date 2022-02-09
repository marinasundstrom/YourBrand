
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TimeReport.Application.Common.Models;

[JsonConverter(typeof(StringEnumConverter))]
public enum SortDirection
{
    Asc = 2,
    Desc = 1
}