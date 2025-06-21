using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace inventory8.Entities
{
    public class StockAudit
    {
        public int Id { get; set; }
        public DateTime Datetime { get; set; }
        public string Notes { get; set; }

        public int HandledBy { get; set; }
        public User User { get; set; }

        public ICollection<StockAuditProduct> StockAuditProducts { get; set; }
    }

}
