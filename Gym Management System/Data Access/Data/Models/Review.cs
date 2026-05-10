namespace Gym_Management_System.Data.Models
{
    public class Review
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User? User { get; set; }

        public Guid? GymClassId { get; set; }
        public GymClass? GymClass { get; set; }

        public Guid TrainerId { get; set; }
        public User? Trainer { get; set; }

        public int Rating { get; set; } 
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
