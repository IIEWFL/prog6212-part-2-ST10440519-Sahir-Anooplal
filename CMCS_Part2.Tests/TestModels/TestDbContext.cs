using Microsoft.EntityFrameworkCore;
using CMCS_Part2.Models;

namespace CMCS_Part2.Tests.TestModels
{
    public class TestDbContext : CMCSDbContext
    {
        public TestDbContext(DbContextOptions<CMCSDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Use in-memory database for testing
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseInMemoryDatabase("TestDatabase");
            }
        }
    }
}