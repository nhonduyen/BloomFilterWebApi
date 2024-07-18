using StackExchange.Redis;
using Newtonsoft.Json;

namespace BloomFilterRedis.Services
{
    public class RedisService : IRedisService
    {
        private readonly IDatabase _redisDb;
        private readonly IConfiguration _config;

        public RedisService(IConnectionMultiplexer redis, IConfiguration config)
        {
            _redisDb = redis.GetDatabase();
            _config = config;
        }

        public Task<bool> StringSetBitAsync(string key, long offset, bool bit)
        {
            return _redisDb.StringSetBitAsync(key, offset, bit);
        }

        public Task<bool> StringGetBitAsync(string key, long offset)
        {
            return _redisDb.StringGetBitAsync(key, offset);
        }

        public Task<bool> StringSetAsync(string key, string value)
        {
            var absoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_config.GetValue<double>("AbsoluteExpiration"));
            return _redisDb.StringSetAsync(key, value, expiry: absoluteExpirationRelativeToNow);
        }

        public Task<bool> SetAsync<T>(string key, T value)
        {
            var absoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_config.GetValue<double>("AbsoluteExpiration"));
            return _redisDb.StringSetAsync(key, JsonConvert.SerializeObject(value), expiry: absoluteExpirationRelativeToNow);
        }

        public async Task<string> StringGetAsync(string key)
        {
            var result = await _redisDb.StringGetAsync(key);
            return result.ToString();
        }

        public Task<bool> KeyExistsAsync(string key)
        {
            return _redisDb.KeyExistsAsync(key);
        }
    }
}
