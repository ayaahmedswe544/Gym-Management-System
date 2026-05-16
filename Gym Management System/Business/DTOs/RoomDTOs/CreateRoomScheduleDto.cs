using Gym_Management_System.Data.Enums;

namespace Gym_Management_System.Business.DTOs.RoomDTOs
{
    public class CreateRoomScheduleDto
    {
        public Guid RoomId { get; set; }
        public Guid TrainerId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ClassType Type { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int MaxCapacity { get; set; }
    }
}
