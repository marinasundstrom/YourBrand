
using System.Text.Json.Serialization;

using Newtonsoft.Json.Converters;

namespace YourBrand.Catalog.Application.Common.Models;

[JsonConverter(typeof(StringEnumConverter))]
public enum SortDirection
{
    Asc = 2,
    Desc = 1
}