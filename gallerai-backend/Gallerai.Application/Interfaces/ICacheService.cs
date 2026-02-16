using Gallerai.Domain.Enums;

namespace Gallerai.Application.Interfaces;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, TimeSpan expiration);
    Task PersistAsync<T>(string key, T value);
    Task RemoveAsync(string key);
    Task<T?> GetOrSetAsync<T>(string key, Func<T> factory, TimeSpan expiration);
    Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan expiration);
    Task<T?> PopAsync<T>(string key);
    Task<bool> TryTransitionStatusAsync(string key, ImageStatus expected, ImageStatus next);
}
