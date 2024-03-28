namespace BlazorApp;

public sealed record PagedResult<T>(IEnumerable<T> Items, int Total);