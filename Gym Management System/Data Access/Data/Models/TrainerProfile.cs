namespace Gym_Management_System.Data.Models
{
    public class TrainerProfile
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User? User { get; set; }

        public string Bio { get; set; } = string.Empty;
        public string Specialties { get; set; } = string.Empty; // Store as comma-separated or JSON
        public int YearsOfExperience { get; set; }
        public string SocialLinks { get; set; } = string.Empty;
    }
}
