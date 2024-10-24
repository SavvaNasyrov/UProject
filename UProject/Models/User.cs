namespace UProject.Models
{
    public class User
    {
        public long Id { get; set; }

        public required string City { get; set; }

        public Interval NotificationInterval { get; set; }
    }
}
