using Newtonsoft.Json;

namespace BloomFilterRedis.Services
{
    public class CacheService
    {
        private readonly IRedisService _redisService;
        private readonly BloomFilterService _bloomFilterService;
        private readonly ILogger<CacheService> _logger;

        public CacheService(IRedisService redisService, BloomFilterService bloomFilterService, ILogger<CacheService> logger)
        {
            _redisService = redisService;
            _bloomFilterService = bloomFilterService;
            _logger = logger;
        }

        public async Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> getFromDb)
        {
            if (!await _bloomFilterService.MightContainAsync(key))
            {
                _logger.LogInformation($"{key} Definitely not in the database");
                return default(T); // Definitely not in the database
            }

            var cachedValue = await _redisService.StringGetAsync(key);
            if (!string.IsNullOrEmpty(cachedValue))
            {
                _logger.LogInformation("Get data from cache");
                T obj = JsonConvert.DeserializeObject<T>(cachedValue);
                return obj;
            }

            var dbValue = await getFromDb();
            if (dbValue != null)
            {
                _logger.LogInformation("Get data from db");
                await _redisService.StringSetAsync(key, JsonConvert.SerializeObject(dbValue));
                await _bloomFilterService.AddToBloomFilterAsync(key);
            }

            return dbValue;
        }

    }
}
