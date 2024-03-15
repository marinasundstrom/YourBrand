namespace YourBrand.StoreFront.Contracts;

public sealed record PagedCartResult
{
    public IEnumerable<Cart> Items { get; init; }
    public int Total { get; init; }
}

public sealed record PagedResult<T>(IEnumerable<T> Items, int Total);