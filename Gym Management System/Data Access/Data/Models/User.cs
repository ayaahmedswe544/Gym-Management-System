using Microsoft.AspNetCore.Identity;

namespace Gym_Management_System.Data.Models
{
    public class User:IdentityUser<Guid>
    {
        public string FullName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
