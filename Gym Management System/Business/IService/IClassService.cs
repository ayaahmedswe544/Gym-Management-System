using Gym_Management_System.Business.DTOs.ClassDTOs;
using Gym_Management_System.Business.GeneralResponse;

namespace Gym_Management_System.Business.IService
{
    public interface IClassService
    {
        Task<GeneralResponse<IEnumerable<ClassDto>>> GetAllClassesAsync();
        Task<GeneralResponse<ClassDto>> GetClassByIdAsync(Guid id);

        Task<GeneralResponse<ClassDto>> CreateClassAsync(CreateClassDto request, Guid trainerId);
        Task<GeneralResponse<ClassDto>> UpdateClassAsync(Guid id, UpdateClassDto request);
        Task<GeneralResponse<bool>> DeleteClassAsync(Guid id);
    }
}
