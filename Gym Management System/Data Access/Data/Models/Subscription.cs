using Gym_Management_System.Data.Enums;

namespace Gym_Management_System.Data.Models
{
    public class Subscription
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public User? User { get; set; }

        public SubscriptionPlan Plan { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public int? ClassesRemaining { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
