using Gym_Management_System.Business.DTOs.ClassDTOs;
using Gym_Management_System.Business.DTOs.RoomDTOs;
using Gym_Management_System.Business.GeneralResponse;

namespace Gym_Management_System.Business.IService
{
    public interface IRoomService
    {
        Task<GeneralResponse<IEnumerable<RoomDto>>> GetRoomsAsync();
        Task<GeneralResponse<RoomDto>> AddRoomAsync(CreateRoomDto roomDto);
        Task<GeneralResponse<IEnumerable<ClassDto>>> GetRoomScheduleAsync(Guid roomId);
        Task<GeneralResponse<bool>> DeleteRoomAsync(Guid id);
        Task<GeneralResponse<RoomDto>> UpdateRoomAsync(Guid id, UpdateRoomDto roomDto);
    }
}
