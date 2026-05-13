using Azure;
using Gym_Management_System.Business.DTOs.TrainerDTOs;
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
    public class TrainersController : ControllerBase
    {
        private readonly ITrainerService _trainerService;

        public TrainersController(ITrainerService trainerService)
        {
            _trainerService = trainerService;
        }

        [HttpGet]
        public async Task<ActionResult<GeneralResponse<List<TrainerProfileDto>>>> GetAllTrainers()
        {
            var response = await _trainerService.GetAllTrainersAsync();
            return StatusCode(response.Success ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GeneralResponse<TrainerDetailsDto>>> GetTrainer(Guid id)
        {
            var response = await _trainerService.GetTrainerAsync(id);   
            return StatusCode(response.Success ? StatusCodes.Status200OK : StatusCodes.Status404NotFound, response);
        }

        [HttpPut("update")]
        [Authorize(Roles = "Trainer")]
        public async Task<ActionResult<GeneralResponse<TrainerProfileDto>>> UpdateProfile([FromForm] UpdateTrainerProfileDto updatedProfile)
        {
            var response = await _trainerService.UpdateProfileAsync(updatedProfile, GetUserId());
            return StatusCode(response.Success ? StatusCodes.Status200OK : StatusCodes.Status500InternalServerError, response);
        }

        [HttpPost("availability")]
        [Authorize(Roles = "Trainer")]
        public async Task<ActionResult<GeneralResponse<TrainerAvailabilityDto>>> SetAvailability([FromBody] CreateTrainerAvailabilityDto availability)
        {
            var response = await _trainerService.SetAvailabilityAsync(availability, GetUserId());
            return StatusCode(response.Success ? StatusCodes.Status200OK : StatusCodes.Status500InternalServerError, response);
        }

        private Guid GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? Guid.Parse(claim.Value) : Guid.Empty;
        }
    }
}
