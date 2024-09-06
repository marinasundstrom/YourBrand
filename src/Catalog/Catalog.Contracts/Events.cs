namespace YourBrand.Catalog.Contracts;

public sealed record ProductDetailsUpdated
{
    public required int ProductId { get; init; }

    public required string Name { get; init; }

    public required string Description { get; init; }
}

public sealed record ProductPriceUpdated
{
    public required int ProductId { get; init; }

    public required decimal NewPrice { get; init; }

    public decimal? RegularPrice { get; init; }

    public double? DiscountRate { get; init; }
}

public sealed record ProductVatRateUpdated
{
    public required int ProductId { get; init; }

    public required double? NewVatRate { get; init; }
}


public sealed record ProductImageUpdated
{
    public required int ProductId { get; init; }

    public required string ImageUrl { get; init; }
}

public sealed record ProductHandleUpdated
{
    public required int ProductId { get; init; }

    public required string Handle { get; init; }
}

public sealed record ProductListingStateUpdated
{
    public required int ProductId { get; init; }

    public required ProductListingState ListingState { get; init; }
}

public enum ProductListingState
{
    Unlisted,
    Listed
}

public sealed record ProductSkuUpdated
{
    public required int ProductId { get; init; }

    public required string Sku { get; init; }
}