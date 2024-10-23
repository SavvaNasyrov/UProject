namespace UProject.Models
{
    public class User
    {
        public required string Id { get; set; }

        public required string City { get; set; }

        public Interval NotificationInterval { get; set; }
    }
}
