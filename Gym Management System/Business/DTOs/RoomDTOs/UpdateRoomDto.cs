namespace Gym_Management_System.Business.DTOs.RoomDTOs
{
    public class UpdateRoomDto
    {
        public string Name { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public string Features { get; set; } = string.Empty;
    }
}
