namespace YourBrand.Sales.Contracts;

public sealed record Cart
{
    public string Id { get; init; }
    public string Tag { get; init; }
    public decimal Total { get; init; }
    public IEnumerable<CartItem> Items { get; init; }
}

public sealed record CartItem
{
    public string Id { get; init; }
    public string Name { get; init; }
    public string? Image { get; init; }
    public long? ProductId { get; init; }
    public string? ProductHandle { get; init; }
    public string Description { get; init; }
    public decimal Price { get; init; }
    public decimal? RegularPrice { get; init; }
    public double Quantity { get; init; }
    public decimal Total { get; init; }
    public string? Data { get; init; }
    public DateTimeOffset Created { get; init; }
    public DateTimeOffset? Updated { get; init; }
}