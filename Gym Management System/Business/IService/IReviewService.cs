using Gym_Management_System.Business.DTOs.ReviewDTOs;
using Gym_Management_System.Business.GeneralResponse;

namespace Gym_Management_System.Business.IService
{
    public interface IReviewService
    {
        Task<GeneralResponse<ReviewDto>> SubmitReviewAsync(CreateReviewDto reviewDto, Guid userId);
        Task<GeneralResponse<ReviewDto>> AddTrainerReviewAsync(CreateReviewDto reviewDto, Guid userId);
        Task<GeneralResponse<IEnumerable<ReviewDto>>> GetClassReviewsAsync(Guid classId);
        Task<GeneralResponse<IEnumerable<ReviewDto>>> GetTrainerReviewsAsync(Guid trainerId);
        Task<GeneralResponse<bool>> DeleteReviewAsync(Guid id);
    }
}
