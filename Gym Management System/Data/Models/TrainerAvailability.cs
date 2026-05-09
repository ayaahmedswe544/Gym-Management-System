namespace Gym_Management_System.Data.Models
{
    public class TrainerAvailability
    {
        public Guid Id { get; set; }
        public Guid TrainerId { get; set; }
        public User? Trainer { get; set; }

        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
