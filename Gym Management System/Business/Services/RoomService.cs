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

        public async Task<GeneralResponse<ClassDto>> AddScheduleToRoomAsync(CreateRoomScheduleDto scheduleDto)
        {
            var gymClass = new GymClass
            {
                RoomId = scheduleDto.RoomId,
                TrainerId = scheduleDto.TrainerId,
                Title = scheduleDto.Title,
                Description = scheduleDto.Description,
                Type = scheduleDto.Type,
                StartTime = scheduleDto.StartTime,
                EndTime = scheduleDto.EndTime,
                MaxCapacity = scheduleDto.MaxCapacity,
                Status = ClassStatus.Scheduled
            };

            await _classRepository.AddAsync(gymClass);
            await _classRepository.SaveChangesAsync();

            var dto = new ClassDto
            {
                Id = gymClass.Id,
                Title = gymClass.Title,
                Description = gymClass.Description,
                Type = gymClass.Type,
                StartTime = gymClass.StartTime,
                MaxCapacity = gymClass.MaxCapacity,
                CurrentBookingsCount = gymClass.CurrentBookingsCount,
                Status = gymClass.Status
            };
            return GeneralResponse<ClassDto>.Ok(dto, "Schedule added to room.");
        }

        public async Task<GeneralResponse<ClassDto>> UpdateScheduleAsync(Guid id, UpdateRoomScheduleDto updatedClassDto)
        {
            var gymClass = await _classRepository.GetByIdAsync(id);
            if (gymClass == null) return GeneralResponse<ClassDto>.Failure("Class not found.");

            gymClass.StartTime = updatedClassDto.StartTime;
            gymClass.EndTime = updatedClassDto.EndTime;
            gymClass.RoomId = updatedClassDto.RoomId;
            gymClass.MaxCapacity = updatedClassDto.MaxCapacity;
            gymClass.Title = updatedClassDto.Title;

            _classRepository.Update(gymClass);
            await _classRepository.SaveChangesAsync();

            var dto = new ClassDto
            {
                Id = gymClass.Id,
                Title = gymClass.Title,
                Description = gymClass.Description,
                Type = gymClass.Type,
                StartTime = gymClass.StartTime,
                MaxCapacity = gymClass.MaxCapacity,
                CurrentBookingsCount = gymClass.CurrentBookingsCount,
                Status = gymClass.Status
            };

            return GeneralResponse<ClassDto>.Ok(dto, "Schedule updated.");
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
