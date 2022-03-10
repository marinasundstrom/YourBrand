
using System.Text.Json.Serialization;

using Newtonsoft.Json.Converters;

namespace Skynet.Showroom.Application.Common.Models;

[JsonConverter(typeof(StringEnumConverter))]
public enum SortDirection
{
    Asc = 2,
    Desc = 1
}