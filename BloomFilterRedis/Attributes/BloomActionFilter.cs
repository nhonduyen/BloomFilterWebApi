using BloomFilterRedis.Data;
using BloomFilterRedis.Services;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace BloomFilterRedis.Attributes
{
    public class BloomActionFilter : IAsyncActionFilter
    {
        private readonly BloomFilterService _filterService;
        private readonly ILogger<BloomActionFilter> _logger;
        private readonly DataContext _dataContext;

        public BloomActionFilter(
            BloomFilterService filterService,
            ILogger<BloomActionFilter> logger,
            DataContext dataContext)
        {
            _filterService = filterService;
            _logger = logger;
            _dataContext = dataContext;
        }


        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (_filterService.BitArray is null || !await _filterService.KeyExistsAsync())
            {
                _logger.LogInformation($"Init bloom filter");
                var emails = await _dataContext.UserProfile.AsNoTracking().Select(x => x.Email).ToListAsync();

                _filterService.InitBloomFilter(emails.Count);
                var tasks = new List<Task>();

                foreach (var email in emails)
                {
                    tasks.Add(_filterService.AddToBloomFilterAsync(email));
                }

                tasks.Add(_filterService.SetBlomfilterCountAsync());
                await Task.WhenAll(tasks);
                _logger.LogInformation($"Finish Init bloom filter with size: {_filterService.BloomFilterSize}");
            }
            else
            {
                var count = await _filterService.GetBlomfilterCountAsync();
                _logger.LogInformation($"Bloom filter already exists - {_filterService.BloomFilterKey} : {count}");
            }
            await next();
        }
    }
}
