

using System.Text.Json;

namespace Microsoft.Extensions.Caching.Distributed;

public static class DistributedCacheExtensions
{
    private static JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
    {
        ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve
    };

    public async static Task SetAsync<T>(this IDistributedCache distributedCache, string key, T value, DistributedCacheEntryOptions options, CancellationToken cancellationToken = default)
    {
        var result = JsonSerializer.Serialize(value, jsonSerializerOptions);
        await distributedCache.SetStringAsync(key, result, options, cancellationToken);
    }

    public async static Task<T> GetAsync<T>(this IDistributedCache distributedCache, string key, CancellationToken cancellationToken = default)
    {
#nullable disable
        var result = await distributedCache.GetStringAsync(key, cancellationToken);
        if (result is not null)
        {
            return JsonSerializer.Deserialize<T>(result, jsonSerializerOptions);
        }
        return default(T);
#nullable restore
    }

    public async static Task<T> GetOrCreateAsync<T>(this IDistributedCache distributedCache, string key, Func<DistributedCacheEntryOptions, Task<T>> factory, CancellationToken cancellationToken = default)
    {
#nullable disable
        var result = await distributedCache.GetAsync<T>(key, cancellationToken);
        if (result is null)
        {
            DistributedCacheEntryOptions options = new();

            result = await factory(options);

            await distributedCache.SetAsync<T>(key, result, options, cancellationToken);
        }
        return result;
#nullable restore
    }
}