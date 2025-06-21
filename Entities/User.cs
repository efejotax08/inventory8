
namespace inventory8.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string UniqueIdentifier { get; set; }
        public string Name { get; set; }
        public string Settings { get; set; }
        public string Permissions { get; set; }
        public ICollection<Attendance> Attendances { get; set; }
        public ICollection<Request> HandledRequests { get; set; }
        public ICollection<StockAudit> HandledAudits { get; set; }

    }

}
