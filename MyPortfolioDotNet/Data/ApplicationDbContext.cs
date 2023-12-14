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
        public DbSet<Technology> Technology { get; set; }   
        public DbSet<ProjectTechnology> ProjectTechnology { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {   
            //one to many project image
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Image>()
            .HasOne(p => p.Project)
            .WithMany(i => i.Images)
            .HasForeignKey(p => p.ProjectId);

            // many to many project technology
            modelBuilder.Entity<ProjectTechnology>()
                .HasKey(pt => new { pt.ProjectId, pt.TechnologyId });

            modelBuilder.Entity<ProjectTechnology>()
                .HasOne(pt => pt.Project)
                .WithMany(p => p.ProjectTechnologies)
                .HasForeignKey(pt => pt.ProjectId);

            modelBuilder.Entity<ProjectTechnology>()
                .HasOne(pt => pt.Technology)
                .WithMany(t => t.ProjectTechnologies)
                .HasForeignKey(pt => pt.TechnologyId);
        }

    }
}
