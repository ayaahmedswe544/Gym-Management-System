namespace Gym_Management_System.Business.DTOs.RoomDTOs
{
    public class UpdateRoomScheduleDto
    {
        public Guid RoomId { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int MaxCapacity { get; set; }
    }
}
