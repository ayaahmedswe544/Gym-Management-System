using Gym_Management_System.Business.DTOs.ReviewDTOs;
using Gym_Management_System.Business.GeneralResponse;
using Gym_Management_System.Business.IService;
using Gym_Management_System.Data.Enums;
using Gym_Management_System.Data.Models;
using Gym_Management_System.Data_Access.IRepository;

namespace Gym_Management_System.Business.Services
{
    public class ReviewService:IReviewService
    {
        private readonly IRepository<Review> _reviewRepository;
        private readonly IRepository<Booking> _bookingRepository;
        private readonly IRepository<GymClass> _classRepository;
        private readonly IRepository<User> _userRepository;

        public ReviewService(IRepository<Review> reviewRepository, IRepository<Booking> bookingRepository, IRepository<GymClass> classRepository, IRepository<User> userRepository)
        {
            _reviewRepository = reviewRepository;
            _bookingRepository = bookingRepository;
            _classRepository = classRepository;
            _userRepository = userRepository;
        }

        public async Task<GeneralResponse<ReviewDto>> SubmitReviewAsync(CreateReviewDto reviewDto, Guid userId)
        {
            if (reviewDto.Rating < 1 || reviewDto.Rating > 5)
            {
                return GeneralResponse<ReviewDto>.Failure("Rating must be between 1 and 5");
            }

            if (reviewDto.GymClassId.HasValue)
            {
                var gymClass = await _classRepository.GetByIdAsync(reviewDto.GymClassId.Value);
                if (gymClass == null)
                {
                    return GeneralResponse<ReviewDto>.Failure("Class not found");
                }

                var attended = await _bookingRepository.FindAsync(b =>
                    b.UserId == userId &&
                    b.GymClassId == reviewDto.GymClassId.Value &&
                    b.Status == BookingStatus.Confirmed);

                if (!attended.Any())
                {
                    return GeneralResponse<ReviewDto>.Failure("You can only review classes you have attended.");
                }

                var existingReview = await _reviewRepository.FindAsync(r =>
                    r.UserId == userId && r.GymClassId == reviewDto.GymClassId.Value);
                if (existingReview.Any())
                {
                    return GeneralResponse<ReviewDto>.Failure("You have already reviewed this class");
                }
            }

            var trainer = await _userRepository.GetByIdAsync(reviewDto.TrainerId);
            if (trainer == null)
            {
                return GeneralResponse<ReviewDto>.Failure("Trainer not found");
            }

            var review = new Review
            {
                UserId = userId,
                GymClassId = reviewDto.GymClassId,
                TrainerId = reviewDto.TrainerId,
                Rating = reviewDto.Rating,
                Comment = reviewDto.Comment,
                CreatedAt = DateTime.UtcNow
            };
            await _reviewRepository.AddAsync(review);
            await _reviewRepository.SaveChangesAsync();
            return GeneralResponse<ReviewDto>.Ok(MapToDto(review), "Review submitted successfully.");
        }

        public async Task<GeneralResponse<ReviewDto>> AddTrainerReviewAsync(CreateReviewDto reviewDto, Guid userId)
        {
            if (reviewDto.Rating < 1 || reviewDto.Rating > 5)
            {
                return GeneralResponse<ReviewDto>.Failure("Rating must be between 1 and 5");
            }

            var trainer = await _userRepository.GetByIdAsync(reviewDto.TrainerId);
            if (trainer == null)
            {
                return GeneralResponse<ReviewDto>.Failure("Trainer not found");
            }

            var myBookings = await _bookingRepository.FindAsync(b => b.UserId == userId && b.Status == BookingStatus.Confirmed);

            bool tookClass = false;
            foreach (var b in myBookings)
            {
                var gymClass = await _classRepository.GetByIdAsync(b.GymClassId);
                if (gymClass != null && gymClass.TrainerId == reviewDto.TrainerId)
                {
                    tookClass = true;
                    break;
                }
            }

            if (!tookClass)
                return GeneralResponse<ReviewDto>.Failure("You can only review trainers whose classes you have attended.");

            var existingReview = await _reviewRepository.FindAsync(r =>
                r.UserId == userId && r.TrainerId == reviewDto.TrainerId);
            if (existingReview.Any())
            {
                return GeneralResponse<ReviewDto>.Failure("You have already reviewed this trainer");
            }

            var review = new Review
            {
                UserId = userId,
                GymClassId = reviewDto.GymClassId,
                TrainerId = reviewDto.TrainerId,
                Rating = reviewDto.Rating,
                Comment = reviewDto.Comment,
                CreatedAt = DateTime.UtcNow
            };
            await _reviewRepository.AddAsync(review);
            await _reviewRepository.SaveChangesAsync();
            return GeneralResponse<ReviewDto>.Ok(MapToDto(review), "Trainer review submitted successfully.");
        }

        public async Task<GeneralResponse<IEnumerable<ReviewDto>>> GetClassReviewsAsync(Guid classId)
        {
            var reviews = await _reviewRepository.FindAsync(r => r.GymClassId == classId);
            var dtos = reviews.Select(MapToDto);
            return GeneralResponse<IEnumerable<ReviewDto>>.Ok(dtos);
        }

        public async Task<GeneralResponse<IEnumerable<ReviewDto>>> GetTrainerReviewsAsync(Guid trainerId)
        {
            var reviews = await _reviewRepository.FindAsync(r => r.TrainerId == trainerId);
            var dtos = reviews.Select(MapToDto);
            return GeneralResponse<IEnumerable<ReviewDto>>.Ok(dtos);
        }

        public async Task<GeneralResponse<bool>> DeleteReviewAsync(Guid id, Guid userId)
        {
            var review = await _reviewRepository.GetByIdAsync(id);
            if (review == null) return new GeneralResponse<bool>()
            {
                Success = false,
                Data = false,
                Message = "Review not found"
            };

            if (review.UserId != userId)
            {
                return new GeneralResponse<bool>()
                {
                    Success = false,
                    Data = false,
                    Message = "You can only delete your own reviews"
                };
            }

            _reviewRepository.Remove(review);
            await _reviewRepository.SaveChangesAsync();
            return new GeneralResponse<bool>()
            {
                Success = true,
                Data = true,
                Message = "Review deleted successfully"
            };
        }

        private ReviewDto MapToDto(Review review)
        {
            return new ReviewDto
            {
                Id = review.Id,
                UserId = review.UserId,
                GymClassId = review.GymClassId,
                TrainerId = review.TrainerId,
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = review.CreatedAt
            };
        }

    }

}
