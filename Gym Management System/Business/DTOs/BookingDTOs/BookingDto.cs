using Gym_Management_System.Data.Enums;

namespace Gym_Management_System.Business.DTOs.BookingDTOs
{
    public class BookingDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid GymClassId { get; set; }
        public BookingStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
