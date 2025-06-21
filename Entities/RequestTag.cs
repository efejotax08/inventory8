using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace inventory8.Entities
{
    public class RequestTag
    {
        public int RequestId { get; set; }
        public Request Request { get; set; }

        public int TagId { get; set; }
        public Tag Tag { get; set; }
    }

}
