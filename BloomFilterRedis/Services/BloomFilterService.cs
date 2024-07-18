using System.Collections;

namespace BloomFilterRedis.Services
{
    public class BloomFilterService
    {
        private readonly IRedisService _redisService;
        public BitArray BitArray { get; set; }
        public string BloomFilterKey { get; } = "bloom_filter_element_count";
        public int ExpectedNumberOfElements { get; set; }
        public int BloomFilterSize { get; set; } = 1000; // Adjust based on your needs
        public int NumberOfHashes { get; set; } = 7; // Adjust based on your needs

        public BloomFilterService(IRedisService redisService)
        {
            _redisService = redisService;
        }

        public async Task AddToBloomFilterAsync(string item)
        {
            for (int i = 0; i < NumberOfHashes; i++)
            {
                int hash = Math.Abs(item.GetHashCode() ^ i) % BloomFilterSize;
                BitArray[hash] = true;
            }
        }

        public async Task<bool> MightContainAsync(string item)
        {
            for (int i = 0; i < NumberOfHashes; i++)
            {
                int hash = Math.Abs(item.GetHashCode() ^ i) % BloomFilterSize;
                if (!BitArray[hash])
                {
                    return false;
                }
            }
            return true;
        }

        public void InitBloomFilter(int expectedNumberOfElements, double desiredFalsePositiveRate = 0.01)
        {
            ExpectedNumberOfElements = expectedNumberOfElements;
            BloomFilterSize = (int)Math.Ceiling(-ExpectedNumberOfElements * Math.Log(desiredFalsePositiveRate) / Math.Pow(Math.Log(2), 2));
            NumberOfHashes = (int)Math.Ceiling((BloomFilterSize / (double)ExpectedNumberOfElements) * Math.Log(2));
            BitArray = new BitArray(BloomFilterSize);
        }

        public async Task<bool> KeyExistsAsync()
        {
            return await _redisService.KeyExistsAsync(BloomFilterKey);
        }

        public async Task<bool> SetBlomfilterCountAsync()
        {
            return await _redisService.StringSetAsync(BloomFilterKey, ExpectedNumberOfElements.ToString());
        }

        public async Task<string> GetBlomfilterCountAsync()
        {
            var value = await _redisService.StringGetAsync(BloomFilterKey);
            return value;
        }
    }
}
