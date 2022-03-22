
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace YourBrand.TimeReport.Application.TimeSheets;

[JsonConverter(typeof(StringEnumConverter))]
public enum TimeSheetStatusDto
{
    Open,
    Closed,
    Approved,
    Disapproved
}