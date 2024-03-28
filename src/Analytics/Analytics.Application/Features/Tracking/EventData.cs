using YourBrand.Analytics.Domain.Enums;

namespace YourBrand.Analytics.Application.Features.Tracking;

public record EventData(EventType EventType, Dictionary<string, object> Data);
