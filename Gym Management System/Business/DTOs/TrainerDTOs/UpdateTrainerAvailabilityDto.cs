namespace Gym_Management_System.Business.DTOs.TrainerDTOs
{
    public class UpdateTrainerAvailabilityDto
    {
        public Guid Id { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

    }
}
