namespace YourBrand.Meetings.Common;

public sealed record ItemsResult<T>(IEnumerable<T> Items, int TotalItems);