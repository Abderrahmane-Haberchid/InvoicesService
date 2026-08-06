namespace Application.Abstractions;

public interface ICacheService
{
    Task<T> GetOrCreateAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        CancellationToken cancellationToken = default);

    Task RemoveAsync(
        string key,
        CancellationToken cancellationToken = default);
}