
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace YourBrand.TimeReport.Features.TimeSheets;

[JsonConverter(typeof(StringEnumConverter))]
public enum EntryStatusDto
{
    Unlocked,
    Locked
}