using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Caching.Distributed;

public static class DistributedCacheExtensions
{
    public async static Task SetAsync<T>(this IDistributedCache distributedCache, string key, T value, DistributedCacheEntryOptions options, CancellationToken token = default)
    {
        var result = JsonSerializer.Serialize(value);
        await distributedCache.SetStringAsync(key, result, options, token);
    }

    public async static Task<T> GetAsync<T>(this IDistributedCache distributedCache, string key, CancellationToken token = default)
    {
#nullable disable
        var result = await distributedCache.GetStringAsync(key, token);
        if (result is not null)
        {
            return JsonSerializer.Deserialize<T>(result);
        }
        return default(T);
#nullable restore
    }
}