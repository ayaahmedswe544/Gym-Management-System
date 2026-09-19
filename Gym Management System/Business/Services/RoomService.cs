using Gym_Management_System.Business.DTOs.ClassDTOs;
using Gym_Management_System.Business.DTOs.RoomDTOs;
using Gym_Management_System.Business.GeneralResponse;
using Gym_Management_System.Business.IService;
using Gym_Management_System.Data.Enums;
using Gym_Management_System.Data.Models;
using Gym_Management_System.Data_Access.IRepository;

namespace Gym_Management_System.Business.Services
{
    public class RoomService:IRoomService
    {
        private readonly IRepository<Room> _roomRepository;
        private readonly IRepository<GymClass> _classRepository;

        public RoomService(IRepository<Room> roomRepository, IRepository<GymClass> classRepository)
        {
            _roomRepository = roomRepository;
            _classRepository = classRepository;
        }

        public async Task<GeneralResponse<IEnumerable<RoomDto>>> GetRoomsAsync()
        {
            var rooms = await _roomRepository.GetAllAsync();
            if (rooms == null)
            {
                return new GeneralResponse<IEnumerable<RoomDto>>
                {
                    Success=true,
                    Message="There are now rooms"

                };
            }
            var data= rooms.Select(r => new RoomDto
            {
                Id = r.Id,
                Name = r.Name,
                Capacity = r.Capacity,
                Features = r.Features
            });
            return new GeneralResponse<IEnumerable<RoomDto>>
            {
                Success = true,
                Message="Rooms pulled successfuly",
                Data=data

            };
        }

        public async Task<GeneralResponse<RoomDto>> AddRoomAsync(CreateRoomDto roomDto)
        {
            var room = new Room
            {
                Name = roomDto.Name,
                Capacity = roomDto.Capacity,
                Features = roomDto.Features
            };
            await _roomRepository.AddAsync(room);
            await _roomRepository.SaveChangesAsync();

            var dto = new RoomDto
            {
                Id = room.Id,
                Name = room.Name,
                Capacity = room.Capacity,
                Features = room.Features
            };
            return GeneralResponse<RoomDto>.Ok(dto, "Room added successfully");
        }

        public async Task<GeneralResponse<IEnumerable<ClassDto>>> GetRoomScheduleAsync(Guid roomId)
        {
            var classes = await _classRepository.FindAsync(c => c.RoomId == roomId && c.StartTime > DateTime.UtcNow);
            if(classes == null)
            {
                return new GeneralResponse<IEnumerable<ClassDto>>
                {
                    Success = true,
                    Message = "There is no schedule for this room"

                };
            }
            var data= classes.OrderBy(c => c.StartTime).Select(c => new ClassDto
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                Type = c.Type,
                StartTime = c.StartTime,
                MaxCapacity = c.MaxCapacity,
                CurrentBookingsCount = c.CurrentBookingsCount,
                Status = c.Status
            });

            return new GeneralResponse<IEnumerable<ClassDto>>
            {
                Success = true,
                Message = "Schedule pulled successfuly",
                Data= data

            };
        }

        public async Task<GeneralResponse<bool>> DeleteRoomAsync(Guid id)
        {
            var room = await _roomRepository.GetByIdAsync(id);
            if (room == null) return GeneralResponse<bool>.Failure("Room not found.");

            _roomRepository.Remove(room);
            await _roomRepository.SaveChangesAsync();

            return GeneralResponse<bool>.Ok(true, "Room deleted successfully.");
        }

        public async Task<GeneralResponse<RoomDto>> UpdateRoomAsync(Guid id, UpdateRoomDto roomDto)
        {
            var room = await _roomRepository.GetByIdAsync(id);
            if (room == null) return GeneralResponse<RoomDto>.Failure("Room not found.");

            room.Name = roomDto.Name;
            room.Capacity = roomDto.Capacity;
            room.Features = roomDto.Features;

            _roomRepository.Update(room);
            await _roomRepository.SaveChangesAsync();

            var dto = new RoomDto
            {
                Id = room.Id,
                Name = room.Name,
                Capacity = room.Capacity,
                Features = room.Features
            };

            return GeneralResponse<RoomDto>.Ok(dto, "Room updated successfully.");
        }
    }
}
