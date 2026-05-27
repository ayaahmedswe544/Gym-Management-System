using Azure;
using Gym_Management_System.Business.DTOs.ReviewDTOs;
using Gym_Management_System.Business.GeneralResponse;
using Gym_Management_System.Business.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Gym_Management_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpPost]
        [Authorize(Roles = "Member")]
        public async Task<ActionResult<GeneralResponse<ReviewDto>>> SubmitReview([FromBody] CreateReviewDto reviewDto)
        {
            var response = await _reviewService.SubmitReviewAsync(reviewDto, GetUserId());
            return StatusCode(response.Success ? StatusCodes.Status201Created : StatusCodes.Status400BadRequest, response);

        }

        [HttpPost("trainer")]
        [Authorize(Roles = "Member")]
        public async Task<ActionResult<GeneralResponse<ReviewDto>>> AddTrainerReview([FromBody] CreateReviewDto reviewDto)
        {
            var response = await _reviewService.AddTrainerReviewAsync(reviewDto, GetUserId());
            return StatusCode(response.Success ? StatusCodes.Status201Created : StatusCodes.Status400BadRequest, response);

        }

        [HttpGet("~/api/classes/{classId}/reviews")]
        public async Task<ActionResult<GeneralResponse<IEnumerable<ReviewDto>>>> GetClassReviews(Guid classId)
        {
            var response = await _reviewService.GetClassReviewsAsync(classId);
            return StatusCode(response.Success ? StatusCodes.Status200OK : StatusCodes.Status404NotFound, response);

        }

        [HttpGet("~/api/trainers/{trainerId}/reviews")]
        public async Task<ActionResult<GeneralResponse<IEnumerable<ReviewDto>>>> GetTrainerReviews(Guid trainerId)
        {
            var response = await _reviewService.GetTrainerReviewsAsync(trainerId);
            return StatusCode(response.Success ? StatusCodes.Status200OK : StatusCodes.Status404NotFound, response);

        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<GeneralResponse>> DeleteReview(Guid id)
        {
            var response = await _reviewService.DeleteReviewAsync(id);
            return StatusCode(response.Success ? StatusCodes.Status200OK : StatusCodes.Status304NotModified, response);

        }

        private Guid GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? Guid.Parse(claim.Value) : Guid.Empty;
        }

    }
}
