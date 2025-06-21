

namespace inventory8.Entities
{
    public class Attendance
    {
        public int Id { get; set; }
        public DateTime Datetime { get; set; }
        public string Notes { get; set; }
        public bool HasEntered { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }

}
