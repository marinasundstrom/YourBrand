namespace YourBrand.Sales.API.Models;

public sealed record PagedResult<T>(IEnumerable<T> Items, int Total);