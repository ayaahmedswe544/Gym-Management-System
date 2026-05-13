using Gym_Management_System.Business.DTOs.ClassDTOs;

namespace Gym_Management_System.Business.DTOs.TrainerDTOs
{
    public class TrainerDetailsDto
    {
        public TrainerProfileDto Profile { get; set; } = new();
        public List<ClassDto> UpcomingClasses { get; set; } = new();
    }
}
