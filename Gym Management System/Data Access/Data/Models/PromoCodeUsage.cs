using Gym_Management_System.Data.Models;

namespace Gym_Management_System.Data_Access.Data.Models
{
    public class PromoCodeUsage
    {
        public Guid Id { get; set; }

        public Guid PromoCodeId { get; set; }
        public PromoCode? PromoCode { get; set; }

        public Guid UserId { get; set; }
        public User? User { get; set; }

        public DateTime UsedAt { get; set; } = DateTime.UtcNow;
    }
}
