using Gym_Management_System.Data.Enums;

namespace Gym_Management_System.Business.DTOs.ClassDTOs
{
    public class ClassDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ClassType Type { get; set; }
        public DateTime StartTime { get; set; }
        public int MaxCapacity { get; set; }
        public int CurrentBookingsCount { get; set; }
        public ClassStatus Status { get; set; }
        public Guid? RoomId { get; set; }
        public Guid TrainerId { get; set; }
        public string TrainerName { get; set; } = string.Empty;
    }
}
