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
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình relationships, indexes, constraints phức tạp
            modelBuilder.Entity<Transaction>(entity =>
            {
                // Có thể override Data Annotation nếu cần
                entity.Property(e => e.Amount).HasPrecision(18, 2);

                // Cấu hình index
                entity.HasIndex(e => e.TransactionDate);

                // Cấu hình relationship
                entity.HasOne(t => t.Category)
                      .WithMany()
                      .HasForeignKey(t => t.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
