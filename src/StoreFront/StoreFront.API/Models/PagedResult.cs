namespace YourBrand.StoreFront.API.Domain.Entities;

public sealed record PagedResult<T>(IEnumerable<T> Items, int Total);