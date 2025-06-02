using FinacialProjectVersion3.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace FinacialProjectVersion3.Data
{
    public class ApplicationDbContext   : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        // DbSet properties for your entities
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        // Add other DbSets as needed
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configure entity properties and relationships here if needed
        }
    }
}
