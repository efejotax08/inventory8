using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace inventory8.Entities
{
    [Table("request_detail")]
    public class RequestDetail
    {
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; }

        public int RequestId { get; set; }
        public Request Request { get; set; }

        public int Amount { get; set; }
    }
    public class RequestDetailDTO
    {
        public ProductDetailDTO Product { get; set; }
        public Supplier Supplier { get; set; }

        public int RequestId { get; set; }
        public int Amount { get; set; }
    }

}
