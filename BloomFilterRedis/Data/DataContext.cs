using BloomFilterRedis.Models;
using Microsoft.EntityFrameworkCore;

namespace BloomFilterRedis.Data
{
    public class DataContext : DbContext
    {
        public DbSet<UserProfile> UserProfile { get; set; }
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

    }
}
