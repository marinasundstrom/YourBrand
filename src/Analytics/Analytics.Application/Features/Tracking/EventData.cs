using System.ComponentModel;
using System.Text.Json.Serialization;

using YourBrand.Analytics.Domain.Enums;

namespace YourBrand.Analytics.Application.Features.Tracking;

[Description("Container for data for an event, its type, and additional data, in the form of key-value pairs.")]
public record EventData(EventType EventType, Dictionary<string, object> Data)
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required EventType EventType { get; set; } = EventType;
}