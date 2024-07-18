using BloomFilterRedis.Attributes;
using BloomFilterRedis.Data;
using BloomFilterRedis.Models;
using BloomFilterRedis.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BloomFilterRedis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(BloomActionFilter))]
    public class ItemController : ControllerBase
    {
        private readonly CacheService _cacheService;
        private readonly DataContext _dataContext;
        private readonly BloomFilterService _bloomFilterService;

        public ItemController(CacheService cacheService, DataContext dataContext, BloomFilterService bloomFilterService)
        {
            _cacheService = cacheService;
            _dataContext = dataContext;
            _bloomFilterService = bloomFilterService;
        }

        [HttpGet("{email}")]
        public async Task<ActionResult<UserProfile>> Email(string email)
        {
            var item = await _cacheService.GetOrAddAsync(email, async () => await _dataContext.UserProfile.FirstOrDefaultAsync(x => x.Email.Equals(email)));
            if (item == null)
            {
                return NotFound();
            }
            return item;
        }
    }
}
