namespace Gym_Management_System.Business.DTOs.TrainerDTOs
{
    public class TrainerProfileDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string FullName { get; set; } = string.Empty; 
        public string Bio { get; set; } = string.Empty;
        public string Specialties { get; set; } = string.Empty;
        public int YearsOfExperience { get; set; }
        public string SocialLinks { get; set; } = string.Empty;
        public string? PhotoUrl { get; set; } = string.Empty;
    }
}
