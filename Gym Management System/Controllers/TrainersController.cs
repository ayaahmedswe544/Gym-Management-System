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

        [HttpPost("add-Availability")]
        [Authorize(Roles = "Trainer")]
        public async Task<ActionResult<GeneralResponse<TrainerAvailabilityDto>>> SetAvailability([FromBody] CreateTrainerAvailabilityDto availability)
        {
            var response = await _trainerService.SetAvailabilityAsync(availability, GetUserId());
            return StatusCode(response.Success ? StatusCodes.Status200OK : StatusCodes.Status500InternalServerError, response);
        }

        [HttpPut("update-availability")]
        [Authorize(Roles = "Trainer")]
        public async Task<ActionResult<GeneralResponse<TrainerAvailabilityDto>>> UpdateAvailability([FromBody] UpdateTrainerAvailabilityDto availability)
        {
            var response = await _trainerService.UpdateAvailabilityAsync(availability, GetUserId());
            
            return StatusCode(response.Success ? StatusCodes.Status200OK : StatusCodes.Status204NoContent, response);
        }
        [HttpDelete("delete-availability/{id}")]
        [Authorize(Roles = "Trainer")]
        public async Task<ActionResult<GeneralResponse<string>>> DeleteAvailability(Guid id)
        {
            var response = await _trainerService.DeleteAvailabilityAsync(id, GetUserId());
            return StatusCode(response.Success ? StatusCodes.Status200OK : StatusCodes.Status500InternalServerError, response);
        }
        [HttpGet("get-availabilies/{trainerId}")]
        public async Task<ActionResult<GeneralResponse<IEnumerable<TrainerAvailabilityDto>>>> GetAvailabilities(Guid trainerId)
        {
            var response = await _trainerService.GetAvailabilitiesAsync(trainerId);
            return StatusCode(response.Success ? StatusCodes.Status200OK : StatusCodes.Status500InternalServerError, response);
        }

        private Guid GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? Guid.Parse(claim.Value) : Guid.Empty;
        }
    }
}
