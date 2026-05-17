using Azure;
using Gym_Management_System.Business.DTOs.ClassDTOs;
using Gym_Management_System.Business.DTOs.RoomDTOs;
using Gym_Management_System.Business.GeneralResponse;
using Gym_Management_System.Business.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Gym_Management_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomService _roomService;

        public RoomsController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        [HttpGet]
        public async Task<ActionResult<GeneralResponse<IEnumerable<RoomDto>>>> GetRooms()
        {
            var response = await _roomService.GetRoomsAsync();
            return StatusCode(response.Success ? StatusCodes.Status200OK : StatusCodes.Status204NoContent, response);
            
        }

        [HttpPost]
        public async Task<ActionResult<GeneralResponse<RoomDto>>> AddRoom([FromBody] CreateRoomDto roomDto)
        {
            var response = await _roomService.AddRoomAsync(roomDto);
            return StatusCode(response.Success ? StatusCodes.Status201Created : StatusCodes.Status400BadRequest, response);
           
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<GeneralResponse<RoomDto>>> UpdateRoom(Guid id, [FromBody] UpdateRoomDto roomDto)
        {
            var response = await _roomService.UpdateRoomAsync(id, roomDto);
            return StatusCode(response.Success ? StatusCodes.Status200OK : StatusCodes.Status304NotModified, response);
        }

        [HttpGet("schedule/{id}")]
        public async Task<ActionResult<GeneralResponse<IEnumerable<ClassDto>>>> GetRoomSchedule(Guid id)
        {
            var response = await _roomService.GetRoomScheduleAsync(id);

            return StatusCode(response.Success ? StatusCodes.Status200OK : StatusCodes.Status404NotFound, response);
        }

        [HttpPost("schedule")]
        public async Task<ActionResult<GeneralResponse<ClassDto>>> AddScheduleToRoom([FromBody] CreateRoomScheduleDto scheduleDto)
        {
            var response = await _roomService.AddScheduleToRoomAsync(scheduleDto);

            return StatusCode(response.Success ? StatusCodes.Status200OK : StatusCodes.Status404NotFound, response);

        }

        [HttpPut("schedule/{id}")]
        public async Task<ActionResult<GeneralResponse<ClassDto>>> UpdateSchedule(Guid id, [FromBody] UpdateRoomScheduleDto updatedClassDto)
        {
            var response = await _roomService.UpdateScheduleAsync(id, updatedClassDto);
            return StatusCode(response.Success ? StatusCodes.Status200OK : StatusCodes.Status304NotModified, response);


        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<GeneralResponse<bool>>> DeleteRoom(Guid id)
        {
            var response = await _roomService.DeleteRoomAsync(id);
            return StatusCode(response.Success ? StatusCodes.Status200OK : StatusCodes.Status304NotModified, response);
        }
    }
}
