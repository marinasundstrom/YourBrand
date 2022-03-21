
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace YourCompany.TimeReport.Application.TimeSheets;

[JsonConverter(typeof(StringEnumConverter))]
public enum EntryStatusDto
{
    Unlocked,
    Locked
}