using Microsoft.AspNetCore.OutputCaching;

public static class OutputCacheOptionsExtensions
{
    public static void AddGetProductsPolicy(this OutputCacheOptions options) => options.AddPolicy(OutputCachePolicyNames.GetProducts, builder =>
    {
        builder.Expire(TimeSpan.FromSeconds(5));
        builder.SetVaryByQuery("brandIdOrHandle", "page", "pageSize", "searchTerm", "categoryPath");
    });

    public static void AddGetProductByIdPolicy(this OutputCacheOptions options) => options.AddPolicy(OutputCachePolicyNames.GetProductById, builder =>
    {
        builder.Expire(TimeSpan.FromSeconds(1));
        builder.SetVaryByQuery("productIdOrHandle");
    });
}