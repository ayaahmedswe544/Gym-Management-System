namespace Gym_Management_System.Business.DTOs.TrainerDTOs
{
    public class TrainerAvailabilityDto
    {
        public Guid Id { get; set; }
        public Guid TrainerId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
