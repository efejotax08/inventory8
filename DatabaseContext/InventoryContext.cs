using inventory8.Entities;
using Microsoft.EntityFrameworkCore;

namespace inventory8.DatabaseContext
{
    public class InventoryContext : DbContext
    {
        public InventoryContext(DbContextOptions<InventoryContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<StockAudit> StockAudits { get; set; }
        public DbSet<StockAuditProduct> StockAuditProducts { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<RequestDetail> RequestDetails { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<ProductTag> ProductTags { get; set; }
        public DbSet<SupplierTag> SupplierTags { get; set; }
        public DbSet<RequestTag> RequestTags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StockAuditProduct>().HasKey(x => new { x.StockAuditId, x.ProductId });
            modelBuilder.Entity<ProductTag>().HasKey(x => new { x.TagId, x.ProductId });
            modelBuilder.Entity<SupplierTag>().HasKey(x => new { x.TagId, x.SupplierId });
            modelBuilder.Entity<RequestTag>().HasKey(x => new { x.TagId, x.RequestId });
            modelBuilder.Entity<RequestDetail>().HasKey(x => new { x.ProductId, x.RequestId });
        }
    }

}
