using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace inventory8.Entities
{
    [Table("request_tag")]
    public class RequestTag
    {
        public int RequestId { get; set; }
        public Request Request { get; set; }

        public int TagId { get; set; }
        public Tag Tag { get; set; }
    }

}
