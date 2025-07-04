using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace inventory8.Entities
{
    [Table("request")]
    public class Request
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Price { get; set; }
        public bool Received { get; set; }
        public string Notes { get; set; }

        public int HandledBy { get; set; }
        [ForeignKey("HandledBy")]
        public User HandledByUser { get; set; }

        public ICollection<RequestDetail> RequestDetails { get; set; }
        public ICollection<RequestTag> RequestTags { get; set; }
    }

    public class RequestDTO
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Price { get; set; }
        public bool Received { get; set; }
        public string Notes { get; set; }
        public UserStockAuditDTO HandledByUser { get; set; }
        
    }



}
