using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace inventory8.Entities
{
    [Table("stock_audit")]
    public class StockAudit
    {
        public int Id { get; set; }
        public DateTime Datetime { get; set; }
        public string Notes { get; set; }
        public int HandledBy { get; set; }
        [ForeignKey(nameof(HandledBy))]
        public User User { get; set; } = new User();


        public ICollection<StockAuditProduct> StockAuditProducts { get; set; } 
    }
    public class StockAuditDTO
    {
        public int Id { get; set; }
        public DateTime Datetime { get; set; }
        public string Notes { get; set; }
        public int HandledBy { get; set; }
        public UserStockAuditDTO User { get; set; } = new UserStockAuditDTO();

    }

    

}
