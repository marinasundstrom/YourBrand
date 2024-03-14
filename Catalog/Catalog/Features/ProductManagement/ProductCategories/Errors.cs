namespace YourBrand.Catalog.Features.ProductManagement.ProductCategories;

public static class Errors
{
    public readonly static Error ProductCategoryNotFound = new("product-category-not-found", "Category not found", "");

    public readonly static Error HandleAlreadyTaken = new("handle-already-taken", "Handle already taken", "");
}