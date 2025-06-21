using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace inventory8.Entities
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<ProductTag> ProductTags { get; set; }
        public ICollection<SupplierTag> SupplierTags { get; set; }
        public ICollection<RequestTag> RequestTags { get; set; }
    }

}
