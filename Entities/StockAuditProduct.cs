using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace inventory8.Entities
{
    [Table("stock_audit_products")]
    public class StockAuditProduct
    {
        public int StockAuditId { get; set; }
        public StockAudit StockAudit { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }
    }

}
