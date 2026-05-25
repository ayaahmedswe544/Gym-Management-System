using Gym_Management_System.Data.Enums;

namespace Gym_Management_System.Business.DTOs.BookingDTOs
{
    public class UpdateBookingDto
    {
        public Guid GymClassId { get; set; }
        public BookingStatus Status { get; set; }
    }
}
