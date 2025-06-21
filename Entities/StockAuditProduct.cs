using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace inventory8.Entities
{
    public class StockAuditProduct
    {
        public int StockAuditId { get; set; }
        public StockAudit StockAudit { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }
    }

}
