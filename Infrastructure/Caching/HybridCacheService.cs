using Application.Abstractions;
using Microsoft.Extensions.Caching.Hybrid;

namespace Infrastructure.Caching;

public class HybridCacheService(HybridCache hybridCache) : ICacheService
{
    public Task<T> GetOrCreateAsync<T>(
        string key, 
        Func<CancellationToken, Task<T>> factory, 
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
    
}