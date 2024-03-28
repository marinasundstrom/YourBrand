namespace YourBrand.StoreFront.API;

public sealed record PagedResult<T>(IEnumerable<T> Items, int Total);