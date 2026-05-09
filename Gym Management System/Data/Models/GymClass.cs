using Gym_Management_System.Data.Enums;

namespace Gym_Management_System.Data.Models
{
    public class GymClass
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ClassType Type { get; set; }

        public Guid TrainerId { get; set; }
        public User? Trainer { get; set; }

        public Guid? RoomId { get; set; }
        public Room? Room { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public int MaxCapacity { get; set; }
        public int CurrentBookingsCount { get; set; }
        public ClassStatus Status { get; set; } = ClassStatus.Scheduled;

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
