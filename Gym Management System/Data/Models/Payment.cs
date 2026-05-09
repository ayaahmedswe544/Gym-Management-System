using Gym_Management_System.Data.Enums;

namespace Gym_Management_System.Data.Models
{
    public class Payment
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public User? User { get; set; }

        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";

        public PaymentType Type { get; set; }
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        public DateTime? PaidAt { get; set; }
    }
}
