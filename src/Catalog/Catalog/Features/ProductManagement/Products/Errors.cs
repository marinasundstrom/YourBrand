namespace YourBrand.Catalog.Features.ProductManagement.Products;

public static class Errors
{
    public readonly static Error ProductNotFound = new("product-not-found", "Product not found", "");

    public readonly static Error ProductPriceExceedsDiscountPrice = new("product-price-exceeds-discount-price", "Product price exceeeds dicount price", "");

    public readonly static Error ProductNotDiscounted = new("product-not-discounted", "Product not discounted", "");

    public readonly static Error ProductAlreadyDiscounted = new("product-already-discounted", "Product already discounted", "");

    public readonly static Error HandleAlreadyTaken = new("handle-already-taken", "Handle already taken", "");

    public readonly static Error SkuAlreadyTaken = new("sku-already-taken", "SKU already taken", "");

    public readonly static Error BrandNotFound = new("brand-not-found", "Brand not found", "");

    public readonly static Error VatRateNotFound = new("vatrate-not-found", "VAT Rate not found", "");
}