using Gym_Management_System.Business.DTOs.TrainerDTOs;
using Gym_Management_System.Business.GeneralResponse;

namespace Gym_Management_System.Business.IService
{
    public interface ITrainerService
    {
        Task<GeneralResponse<List<TrainerProfileDto>>> GetAllTrainersAsync();
        Task<GeneralResponse<TrainerDetailsDto>> GetTrainerAsync(Guid id);
        Task<GeneralResponse<TrainerProfileDto>> UpdateProfileAsync(UpdateTrainerProfileDto updatedProfile, Guid userId);
        Task<GeneralResponse<TrainerAvailabilityDto>> SetAvailabilityAsync(CreateTrainerAvailabilityDto availability, Guid trainerId);
        Task<GeneralResponse<TrainerAvailabilityDto>> UpdateAvailabilityAsync(UpdateTrainerAvailabilityDto availability, Guid trainerId);
        Task<GeneralResponse<string>> DeleteAvailabilityAsync(Guid availabilityId, Guid trainerId);
        Task<GeneralResponse<IEnumerable<TrainerAvailabilityDto>>> GetAvailabilitiesAsync(Guid trainerId);
    }
}
