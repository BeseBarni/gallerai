using System.Text.Json;
using Gallerai.Application.Interfaces;
using StackExchange.Redis;

namespace Gallerai.Infrastructure.Services;

internal sealed class RedisCacheService : ICacheService
{
    private readonly IConnectionMultiplexer redis;
    private readonly IDatabase db;

    private const string CacheKeyPrefix = "GalleraiCache:";

    private string CreateKey(string key) => $"{CacheKeyPrefix}{key}";

    public RedisCacheService(IConnectionMultiplexer redis)
    {
        this.redis = redis;
        db = redis.GetDatabase();
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var value = await db.StringGetAsync(CreateKey(key));

        if (value.IsNullOrEmpty) return default;

        return System.Text.Json.JsonSerializer.Deserialize<T>(value.ToString());
    }

    public async Task RemoveAsync(string key)
    {
        await db.KeyDeleteAsync(CreateKey(key));
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan expiration)
    {
        var json = JsonSerializer.Serialize(value);

        await db.StringSetAsync(CreateKey(key), json, expiration);
    }

    public async Task PersistAsync<T>(string key, T value)
    {
        var json = JsonSerializer.Serialize(value);
        var redisKey = CreateKey(key);

        await db.StringSetAsync(redisKey, json);
        await db.KeyPersistAsync(redisKey);
    }

    public async Task<T?> GetOrSetAsync<T>(string key, Func<T> factory, TimeSpan expiration)
    {
        var cached = await GetAsync<T>(key);

        if (cached is not null) return cached;

        var value = factory();

        if (value is not null)
        {
            await SetAsync(key, value, expiration);
        }

        return value;
    }

    public async Task<T?> PopAsync<T>(string key)
    {
        var value = await db.StringGetDeleteAsync(CreateKey(key));

        return value.HasValue ? JsonSerializer.Deserialize<T>(value.ToString()) : default;
    }
}
