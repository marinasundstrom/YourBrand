using Microsoft.AspNetCore.OutputCaching;

public static class OutputCacheOptionsExtensions
{
    public static void AddGetProductsPolicy(this OutputCacheOptions options) => options.AddPolicy(OutputCachePolicyNames.GetProducts, builder =>
    {
        builder.Expire(TimeSpan.FromSeconds(0.6));
        builder.SetVaryByQuery("storeId", "brandIdOrHandle", "includeUnlisted", "groupProducts", "searchTerm", "categoryPathOrId", "page", "pageSize", "sortBy", "sortDirection");
    });

    public static void AddGetProductByIdPolicy(this OutputCacheOptions options) => options.AddPolicy(OutputCachePolicyNames.GetProductById, builder =>
    {
        builder.Expire(TimeSpan.FromSeconds(1));
        builder.SetVaryByQuery("idOrHandle");
    });
}