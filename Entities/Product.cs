

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
        public List<int> TagIds { get; set; } = new();
    }
    public class ExtendedProductDetailDTO
    {
        public DateTime LastAudit { get; set; }
        public decimal AcquisitionPrice { get; set; }
        public bool SubscribeToInventory { get; set; }
        public string PackagingUnit { get; set; } = null!;
        public string Stats { get; set; } = null!;
        public SupplierDetailDTO Supplier { get; set; }     
        public List<RequestDTO> Requests { get; set; } = new();
        public List<StockAuditDTO> Audits { get; set; }= new();
    }

    public class ProductDetailDTO
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
        public List<ProductTagDto> ProductTags { get; set; } = new();
    }
    public class ProductTagDto
    {
        public int TagId { get; set; }
        public string TagName { get; set; } = string.Empty;
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
