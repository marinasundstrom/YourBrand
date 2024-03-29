namespace YourBrand.Sales.Models;

public sealed record PagedResult<T>(IEnumerable<T> Items, int Total);