using Microsoft.EntityFrameworkCore;
using MyPortfolioDotNet.Models;

namespace MyPortfolioDotNet.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Project> Project { get; set; }
    }
}
