using Application.Abstractions;
using Microsoft.Extensions.Caching.Hybrid;

namespace Infrastructure.Caching;

public class HybridCacheService(HybridCache hybridCache) : ICacheService
{
    public async Task<T> GetOrCreateAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        CancellationToken cancellationToken = default)
    {
        return await hybridCache.GetOrCreateAsync(
            key,
            async ct => await factory(ct),
            cancellationToken: cancellationToken);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await hybridCache.RemoveAsync(key, cancellationToken);
    }
}