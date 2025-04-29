using RedisProductAPI.Infrastructure.Contract;
using StackExchange.Redis;
using System.Text.Json;

namespace RedisProductAPI.Infrastructure.Cache;
public class RedisCacheService : IRedisCacheService
{
    private readonly IDatabase _cacheDb;

    public RedisCacheService()
    {
        var redis = ConnectionMultiplexer.Connect("localhost:6379"); 
        _cacheDb = redis.GetDatabase();
    }

    public async Task<T?> GetDataAsync<T>(string key)
    {
        var value = await _cacheDb.StringGetAsync(key);
        if (!string.IsNullOrEmpty(value))
        {
            return JsonSerializer.Deserialize<T>(value);
        }
        return default;
    }

    public async Task SetDataAsync<T>(string key, T value, TimeSpan expiration)
    {
        var serializedData = JsonSerializer.Serialize(value);
        await _cacheDb.StringSetAsync(key, serializedData, expiration);
    }

    public async Task RemoveDataAsync(string key)
    {
        await _cacheDb.KeyDeleteAsync(key);
    }

    public async Task UpdateDataAsyn<T>(string key, T value, TimeSpan expiration){
        await _cacheDb.KeyDeleteAsync(key);
        var serializedData = JsonSerializer.Serialize(value);
        await _cacheDb.StringSetAsync(key, serializedData, expiration);
    }

}

