using Microsoft.EntityFrameworkCore;
namespace PIMS.EntityFramework.Models
{
    public class PimsDbContext : DbContext
    {
        public PimsDbContext()
        {

        }
        public PimsDbContext(DbContextOptions<PimsDbContext> options) : base(options)
        {
        }

        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Inventory> Inventories { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<InventoryTransaction> InventoryTransactions { get; set; }
        public virtual DbSet<ProductCategory> ProductCategories { get; set; }
        public virtual DbSet<Usersession> Usersessions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductCategory>()
                .HasKey(pc => new { pc.ProductId, pc.CategoryId });

            modelBuilder.Entity<ProductCategory>()
                .HasOne(pc => pc.Product)
                .WithMany(p => p.ProductCategories)
                .HasForeignKey(pc => pc.ProductId);

            modelBuilder.Entity<ProductCategory>()
                .HasOne(pc => pc.Category)
                .WithMany(c => c.ProductCategories)
                .HasForeignKey(pc => pc.CategoryId);

            modelBuilder.Entity<InventoryTransaction>()
                .HasOne(it => it.Inventory)
                .WithMany()
                .HasForeignKey(it => it.InventoryId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Usersession>()
                .HasKey(us => us.TokenId);  // Corrected primary key

            modelBuilder.Entity<Usersession>()
                .HasOne(us => us.User)
                .WithMany()
                .HasForeignKey(us => us.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}