

using System.ComponentModel.DataAnnotations.Schema;

namespace inventory8.Entities
{
    public class ProductDTO
    {
        public string ProductId { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int StockQuantity { get; set; }
        public int LowStockThreshold { get; set; }
        public decimal AcquisitionPrice { get; set; }
        public string? PhotoUrl { get; set; }
        public bool SubscribeToInventory { get; set; }
        public string PackagingUnit { get; set; } = null!;
        //public string Stats { get; set; } = null!;
        public int SupplierId { get; set; }
    }


    [Table("products")]
    public class Product
    {
        public int Id { get; set; }
        public string ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? LastAudit { get; set; }
        public int StockQuantity { get; set; }
        public int LowStockThreshold { get; set; }
        public decimal AcquisitionPrice { get; set; }
        public string? PhotoUrl { get; set; }
        public bool SubscribeToInventory { get; set; }
        public string PackagingUnit { get; set; }
        public string Stats { get; set; } 

        public int SupplierId { get; set; }
        public Supplier? Supplier { get; set; }

        public ICollection<StockAuditProduct> StockAuditProducts { get; set; } = new List<StockAuditProduct>();
        public ICollection<ProductTag> ProductTags { get; set; } = new List<ProductTag>();
        public ICollection<RequestDetail> RequestDetails { get; set; } = new List<RequestDetail>();
    }


}
