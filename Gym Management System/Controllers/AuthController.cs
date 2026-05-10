using Gym_Management_System.Business.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Gym_Management_System.Business.DTOs.AuthDTOs;

namespace Gym_Management_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register/member")]
        public async Task<IActionResult> RegisterMember([FromBody] RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request, "Member");
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("register/trainer")]
        public async Task<IActionResult> RegisterTrainer([FromBody] RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request, "Trainer");
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("register/admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request, "Admin");
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);
            if (!result.Success) return Unauthorized(result);
            return Ok(result);
        }
    }
}
