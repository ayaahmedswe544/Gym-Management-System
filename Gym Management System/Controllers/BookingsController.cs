using Azure;
using Gym_Management_System.Business.DTOs.BookingDTOs;
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
    [Authorize]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpPost]
        [Authorize(Roles = "Member,Admin")] 
        public async Task<ActionResult<GeneralResponse<BookingDto>>> CreateBooking([FromBody] CreateBookingDto request)
        {
            var userId = GetUserId();
            var response = await _bookingService.CreateBookingAsync(request, userId);
            return StatusCode(response.Success ? StatusCodes.Status201Created : StatusCodes.Status400BadRequest, response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<GeneralResponse<string>>> CancelBooking(Guid id)
        {
            var userId = GetUserId();
            var response = await _bookingService.CancelBookingAsync(id, userId);
            return StatusCode(response.Success ? StatusCodes.Status200OK : StatusCodes.Status404NotFound, response);
        }

        [HttpGet("my")]
        public async Task<ActionResult<GeneralResponse<IEnumerable<BookingDto>>>> GetMyBookings()
        {
            var userId = GetUserId();
            var response = await _bookingService.GetMyBookingsAsync(userId);
            return StatusCode(response.Success ? StatusCodes.Status200OK : StatusCodes.Status404NotFound, response);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Trainer")]
        public async Task<ActionResult<GeneralResponse<BookingDto>>> UpdateBooking(Guid id, [FromBody] UpdateBookingDto request)
        {
            var response = await _bookingService.UpdateBookingAsync(id, request);

            return StatusCode(response.Success ? StatusCodes.Status200OK : StatusCodes.Status304NotModified, response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GeneralResponse<BookingDto>>> GetBooking(Guid id)
        {
            var response = await _bookingService.GetBookingByIdAsync(id);
            return StatusCode(response.Success ? StatusCodes.Status200OK : StatusCodes.Status404NotFound, response);
        }

        [HttpGet("user/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<GeneralResponse<IEnumerable<BookingDto>>>> GetUserBookings(Guid userId)
        {
            var response = await _bookingService.GetBookingsByUserIdAsync(userId);
            return StatusCode(response.Success ? StatusCodes.Status200OK : StatusCodes.Status404NotFound, response);
        }

        private Guid GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return Guid.Parse(userIdClaim!.Value);
        }
    }
}
