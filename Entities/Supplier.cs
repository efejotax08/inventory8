﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace inventory8.Entities
{
    [Table("suppliers")]
    public class Supplier
    {
        public int Id { get; set; }
        public string UniqueIdentifier { get; set; }
        public string Name { get; set; }
        public string Contact { get; set; }

        public ICollection<Product> Products { get; set; }
        public ICollection<SupplierTag> SupplierTags { get; set; }
    }
    public class SupplierDetailDTO
    {
        public int Id { get; set; }
        public string UniqueIdentifier { get; set; } 
        public string? Name { get; set; }
        public string? Contact { get; set; }
    }

}
