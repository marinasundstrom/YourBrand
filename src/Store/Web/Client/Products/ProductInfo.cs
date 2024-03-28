namespace Client.Products;

public sealed record ProductInfo(string Name, string? Image, long? ProductId, string? Handle, string Description, decimal Price, decimal? RegularPrice);