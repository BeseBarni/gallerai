using System.Text.Json;
using Gallerai.Application.Interfaces;
using Gallerai.Domain.Enums;
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

    public async Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan expiration)
    {
        var cached = await GetAsync<T>(key);

        if (cached is not null) return cached;

        var value = await factory();

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

    public async Task<bool> TryTransitionStatusAsync(string key, ImageStatus expected, ImageStatus next)
    {
        // Lua script: 
        // "If the value at KEY matches ARGV[1], set it to ARGV[2] and return 1 (true). 
        //  Otherwise return 0 (false)."
        key = CreateKey(key);
        var script = @"
        local current = redis.call('get', KEYS[1])
    
        -- If key is missing (not current) OR key matches expected status
        if not current or current == ARGV[1] then
            redis.call('set', KEYS[1], ARGV[2], 'KEEPTTL')
            return 1
        else
            return 0
        end";

        var result = await db.ScriptEvaluateAsync(
            script,
            [key],
            [(int)expected, (int)next]
        );

        var success = (int)result == 1;

        return success;
    }
}
