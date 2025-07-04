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
            modelBuilder.Entity<ProductTag>().HasKey(x => new { x.ProductId ,x.TagId });
            modelBuilder.Entity<SupplierTag>().HasKey(x => new { x.TagId, x.SupplierId });
            modelBuilder.Entity<RequestTag>().HasKey(x => new { x.TagId, x.RequestId });
            modelBuilder.Entity<RequestDetail>().HasKey(x => new { x.ProductId, x.RequestId });


            // Relaciones ProductTag
            modelBuilder.Entity<ProductTag>()
                .HasOne(pt => pt.Product)
                .WithMany(p => p.ProductTags)
                .HasForeignKey(pt => pt.ProductId);

            modelBuilder.Entity<ProductTag>()
                .HasOne(pt => pt.Tag)
                .WithMany(t => t.ProductTags)
                .HasForeignKey(pt => pt.TagId);

            // Relaciones RequestTag
            modelBuilder.Entity<RequestTag>()
                .HasOne(rt => rt.Request)
                .WithMany(r => r.RequestTags)
                .HasForeignKey(rt => rt.RequestId);

            modelBuilder.Entity<RequestTag>()
                .HasOne(rt => rt.Tag)
                .WithMany(t => t.RequestTags)
                .HasForeignKey(rt => rt.TagId);

            // Relaciones SupplierTag
            modelBuilder.Entity<SupplierTag>()
                .HasOne(st => st.Supplier)
                .WithMany(s => s.SupplierTags)
                .HasForeignKey(st => st.SupplierId);

            modelBuilder.Entity<SupplierTag>()
                .HasOne(st => st.Tag)
                .WithMany(t => t.SupplierTags)
                .HasForeignKey(st => st.TagId);

            // Relaciones Request → User
            modelBuilder.Entity<Request>()
                .HasOne(r => r.HandledByUser)
                .WithMany()
                .HasForeignKey(r => r.HandledBy)
                .OnDelete(DeleteBehavior.Restrict);

            // Relaciones StockAudit → User
            modelBuilder.Entity<StockAudit>()
                .HasOne(sa => sa.HandledByUser)
                .WithMany()
                .HasForeignKey(sa => sa.HandledBy)
                .OnDelete(DeleteBehavior.Restrict);

            // Relaciones RequestDetail → Request, Product, Supplier
            modelBuilder.Entity<RequestDetail>()
                .HasOne(rd => rd.Product)
                .WithMany(p => p.RequestDetails)
                .HasForeignKey(rd => rd.ProductId);

            modelBuilder.Entity<RequestDetail>()
                .HasOne(rd => rd.Request)
                .WithMany(r => r.RequestDetails)
                .HasForeignKey(rd => rd.RequestId);

            modelBuilder.Entity<RequestDetail>()
                .HasOne(rd => rd.Supplier)
                .WithMany()
                .HasForeignKey(rd => rd.SupplierId);

            // Relaciones Product → Supplier
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Supplier)
                .WithMany(s => s.Products)
                .HasForeignKey(p => p.SupplierId);

            // Relaciones StockAuditProduct → Product y StockAudit
            modelBuilder.Entity<StockAuditProduct>()
                .HasOne(sap => sap.Product)
                .WithMany(p => p.StockAuditProducts)
                .HasForeignKey(sap => sap.ProductId);

            modelBuilder.Entity<StockAuditProduct>()
                .HasOne(sap => sap.StockAudit)
                .WithMany(sa => sa.StockAuditProducts)
                .HasForeignKey(sap => sap.StockAuditId);
        }
    }

}
