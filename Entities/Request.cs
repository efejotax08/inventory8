using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace inventory8.Entities
{
    public class Request
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Price { get; set; }
        public bool Received { get; set; }
        public string Notes { get; set; }

        public int HandledBy { get; set; }
        public User User { get; set; }

        public ICollection<RequestDetail> RequestDetails { get; set; }
        public ICollection<RequestTag> RequestTags { get; set; }
    }

}
