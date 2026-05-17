using Azure;
using Gym_Management_System.Business.DTOs.ClassDTOs;
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
    public class ClassesController : ControllerBase
    {
        private readonly IClassService _classService;

        public ClassesController(IClassService classService)
        {
            _classService = classService;
        }

        [HttpGet]
        public async Task<ActionResult<GeneralResponse<IEnumerable<ClassDto>>>> GetClasses()
        {
            var response = await _classService.GetAllClassesAsync();
            return StatusCode(response.Success ? StatusCodes.Status200OK : StatusCodes.Status404NotFound, response);

        }

        [HttpPost]
        [Authorize(Roles = "Admin,Trainer")]
        public async Task<ActionResult<GeneralResponse<ClassDto>>> CreateClass([FromBody] CreateClassDto request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized();

            var userId = Guid.Parse(userIdClaim.Value);
            var isAdmin = User.IsInRole("Admin");
            
            Guid trainerId;
            if (isAdmin)
            {
                if (!request.TrainerId.HasValue)
                {
                    var response1 = new GeneralResponse<ClassDto>
                    {
                        Success=false,
                        Data=null,
                        Message= "Admin must specify a TrainerId when creating a class",
                        Errors = new Dictionary<string, string[]>
                        {
                           { "TrainerId", new string[]{ "trainer id can't be null" } }
                        }

                    };
                    return StatusCode(StatusCodes.Status400BadRequest, response1);
                }
                trainerId = request.TrainerId.Value;
            }
            else
            {
                trainerId = userId;
            }


            var response = await _classService.CreateClassAsync(request, trainerId);

            return StatusCode(response.Success ? StatusCodes.Status201Created : StatusCodes.Status400BadRequest, response);

        }
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Trainer")]
        public async Task<ActionResult<GeneralResponse<ClassDto>>> UpdateClass(Guid id, [FromBody] UpdateClassDto request)

        {


            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized();

            var userId = Guid.Parse(userIdClaim.Value);
            var isAdmin = User.IsInRole("Admin");

            Guid trainerId;
            if (isAdmin)
            {
                if (!request.TrainerId.HasValue)
                {
                    var response1 = new GeneralResponse<ClassDto>
                    {
                        Success = false,
                        Data = null,
                        Message = "Admin must specify a TrainerId when updating a class",
                        Errors = new Dictionary<string, string[]>
                        {
                           { "TrainerId", new string[]{ "trainer id can't be null" } }
                        }

                    };
                    return StatusCode(StatusCodes.Status400BadRequest, response1);
                }
                trainerId = request.TrainerId.Value;
            }
            else
            {
                trainerId = userId;
            }

            var response = await _classService.UpdateClassAsync(id, request);

            return StatusCode(response.Success ? StatusCodes.Status200OK : StatusCodes.Status304NotModified, response);

        }
        [HttpGet("{id}")]
        public async Task<ActionResult<GeneralResponse<ClassDto>>> GetClass(Guid id)
        {
            var response = await _classService.GetClassByIdAsync(id);
            return StatusCode(response.Success ? StatusCodes.Status200OK : StatusCodes.Status404NotFound, response);

        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Trainer")]
        public async Task<ActionResult<GeneralResponse<bool>>> DeleteClass(Guid id)
        {
            var response = await _classService.DeleteClassAsync(id);

            return StatusCode(response.Success ? StatusCodes.Status200OK : StatusCodes.Status304NotModified, response);

        }
    }
}
