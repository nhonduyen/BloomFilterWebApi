namespace BloomFilterRedis.Services
{
    public interface IRedisService
    {
        Task<bool> StringSetBitAsync(string key, long offset, bool bit);
        Task<bool> StringGetBitAsync(string key, long offset);
        Task<bool> StringSetAsync(string key, string value);
        Task<bool> SetAsync<T>(string key, T value);
        Task<string> StringGetAsync(string key);
        Task<bool> KeyExistsAsync(string key);
    }
}
