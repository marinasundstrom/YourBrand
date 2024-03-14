namespace YourBrand.Catalog.Model;

public sealed record PagedResult<T>(IEnumerable<T> Items, int Total);