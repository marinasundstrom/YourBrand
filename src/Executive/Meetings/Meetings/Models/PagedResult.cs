namespace YourBrand.Meetings.Models;

public sealed record PagedResult<T>(IEnumerable<T> Items, int Total);