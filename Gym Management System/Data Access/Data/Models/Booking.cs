using Gym_Management_System.Data.Enums;

namespace Gym_Management_System.Data.Models
{
    public class Booking
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public User? User { get; set; }

        public Guid GymClassId { get; set; }
        public GymClass? GymClass { get; set; }

        public BookingStatus Status { get; set; } = BookingStatus.Pending;

        public Guid? PaymentId { get; set; }
        public Payment? Payment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CancelledAt { get; set; }
    }
}
