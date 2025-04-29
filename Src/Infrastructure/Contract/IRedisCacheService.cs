namespace RedisProductAPI.Infrastructure.Contract;
public interface IRedisCacheService
{
    Task<T?> GetDataAsync<T>(string key);
    Task SetDataAsync<T>(string key, T value,TimeSpan expiration);
    Task RemoveDataAsync(string key);
    Task UpdateDataAsyn<T>(string key, T value, TimeSpan expiration);
}
