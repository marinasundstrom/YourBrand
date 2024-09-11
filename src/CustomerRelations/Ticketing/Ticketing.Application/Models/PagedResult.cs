namespace YourBrand.Ticketing.Models;

public sealed record PagedResult<T>(IEnumerable<T> Items, int Total);