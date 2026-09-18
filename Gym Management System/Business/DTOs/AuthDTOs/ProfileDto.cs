namespace Gym_Management_System.Business.DTOs.AuthDTOs
{
    public class ProfileDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string Role { get; set; } = string.Empty;
        public bool IsTrainer { get; set; }
        public TrainerProfileInfo? TrainerProfile { get; set; }
    }

    public class TrainerProfileInfo
    {
        public string Bio { get; set; } = string.Empty;
        public string Specialties { get; set; } = string.Empty;
        public int YearsOfExperience { get; set; }
        public string SocialLinks { get; set; } = string.Empty;
        public string PhotoUrl { get; set; } = string.Empty;
    }
}
