namespace Gym_Management_System.Business.DTOs.TrainerDTOs
{
    public class UpdateTrainerProfileDto
    {
        public string? Bio { get; set; } = string.Empty;
        public string? Specialties { get; set; } = string.Empty;
        public int? YearsOfExperience { get; set; }
        public string? SocialLinks { get; set; } = string.Empty;
        public IFormFile? Photo { get; set; }
    }
}
