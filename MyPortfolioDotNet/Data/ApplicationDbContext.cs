using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyPortfolioDotNet.Models;

namespace MyPortfolioDotNet.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Project> Project { get; set; }
        public DbSet<Image> Image { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Image>()
            .HasOne(p => p.Project)
            .WithMany(i => i.Images)
            .HasForeignKey(p => p.ProjectId);
        }

    }
}
