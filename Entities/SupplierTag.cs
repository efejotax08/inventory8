using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace inventory8.Entities
{
    public class SupplierTag
    {
        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; }

        public int TagId { get; set; }
        public Tag Tag { get; set; }
    }

}
