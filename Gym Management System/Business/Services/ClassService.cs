using Gym_Management_System.Business.DTOs.ClassDTOs;
using Gym_Management_System.Business.GeneralResponse;
using Gym_Management_System.Business.IService;
using Gym_Management_System.Data.Enums;
using Gym_Management_System.Data.Models;
using Gym_Management_System.Data_Access.IRepository;

namespace Gym_Management_System.Business.Services
{
    public class ClassService:IClassService
    {
        private readonly IRepository<GymClass> _repository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Room> _roomRepository;

        public ClassService(IRepository<GymClass> repository, IRepository<User> userRepository, IRepository<Room> roomRepository)
        {
            _repository = repository;
            _userRepository = userRepository;
            _roomRepository = roomRepository;
        }

        public async Task<GeneralResponse<IEnumerable<ClassDto>>> GetAllClassesAsync()
        {
            var classes = await _repository.GetAllWithIncludesAsync(c => c.Trainer!);
            if (classes == null)
            {
                  return new GeneralResponse<IEnumerable<ClassDto>>
                {
                    Success = true,
                    Message = "There are no classes"

                };
            }
            var data =classes.Select(c => new ClassDto
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                Type = c.Type,
                StartTime = c.StartTime,
                EndTime= c.EndTime,
                MaxCapacity = c.MaxCapacity,
                CurrentBookingsCount = c.CurrentBookingsCount,
                Status = c.Status,
                RoomId= c.RoomId,
               TrainerId = c.TrainerId,
                TrainerName = c.Trainer?.FullName ?? string.Empty
            });
            return new GeneralResponse<IEnumerable<ClassDto>>
            {
                Success = true,
                Data=data,
                Message = "Classes pulled successfuly"

            };


        }
        public async Task<GeneralResponse<ClassDto>> GetClassByIdAsync(Guid id)
        {
            var gymClass = await _repository.GetByIdWithIncludesAsync(id, c => c.Trainer!);
            if (gymClass == null)
            {
                return new GeneralResponse<ClassDto>
                {
                    Success = false,
                    Message = "This class doesn't exist"

                };
            }
                

            return GeneralResponse<ClassDto>.Ok(new ClassDto
            {
                Id = gymClass.Id,
                Title = gymClass.Title,
                Description = gymClass.Description,
                Type = gymClass.Type,
                StartTime = gymClass.StartTime,
                EndTime = gymClass.EndTime, 
                MaxCapacity = gymClass.MaxCapacity,
                CurrentBookingsCount = gymClass.CurrentBookingsCount,
                Status = gymClass.Status,
                RoomId = gymClass.RoomId,
                TrainerId = gymClass.TrainerId,
                TrainerName = gymClass.Trainer?.FullName ?? string.Empty
            });
        }

        public async Task<GeneralResponse<ClassDto>> CreateClassAsync(CreateClassDto request, Guid trainerId)
        {
            var trainer = await _userRepository.GetByIdAsync(trainerId);
            if (trainer == null)
            {
                return GeneralResponse<ClassDto>.Failure("Trainer not found");
            }

            if (request.StartTime >= request.EndTime)
            {
                return GeneralResponse<ClassDto>.Failure("Start time must be before end time");
            }

            if (request.MaxCapacity <= 0)
            {
                return GeneralResponse<ClassDto>.Failure("Max capacity must be greater than zero");
            }

            if (request.RoomId.HasValue)
            {
                var room = await _roomRepository.GetByIdAsync(request.RoomId.Value);
                if (room == null)
                {
                    return GeneralResponse<ClassDto>.Failure("Room not found");
                }

                if (room.Capacity < request.MaxCapacity)
                {
                    return GeneralResponse<ClassDto>.Failure($"Room capacity ({room.Capacity}) is less than class max capacity ({request.MaxCapacity})");
                }

                var hasConflict = await HasRoomConflictAsync(request.RoomId.Value, request.StartTime, request.EndTime);
                if (hasConflict)
                {
                    return GeneralResponse<ClassDto>.Failure("Room is already booked for another class at this time");
                }
            }

            var trainerConflict = await GetTrainerConflictAsync(trainerId, request.StartTime, request.EndTime);
            if (trainerConflict != null)
            {
                return GeneralResponse<ClassDto>.FailureWithData($"Trainer is already scheduled for another class at this time: {trainerConflict.Title}", trainerConflict);
            }

            var gymClass = new GymClass
            {
                Title = request.Title,
                Description = request.Description,
                Type = request.Type,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                MaxCapacity = request.MaxCapacity,
                TrainerId = trainerId,
                Status = ClassStatus.Scheduled,
                RoomId = request.RoomId
            };

            await _repository.AddAsync(gymClass);
            await _repository.SaveChangesAsync();

            return GeneralResponse<ClassDto>.Ok(new ClassDto
            {
                Id = gymClass.Id,
                Title = gymClass.Title,
                Description = gymClass.Description,
                Type = gymClass.Type,
                StartTime = gymClass.StartTime,
                EndTime = gymClass.EndTime,
                MaxCapacity = gymClass.MaxCapacity,
                CurrentBookingsCount = gymClass.CurrentBookingsCount,
                Status = gymClass.Status,
                RoomId = gymClass.RoomId,
                TrainerId = gymClass.TrainerId,
                TrainerName = trainer.FullName ?? string.Empty
            }, "Class created successfully");
        }
        public async Task<GeneralResponse<ClassDto>> UpdateClassAsync(Guid id, UpdateClassDto request)
        {
            var gymClass = await _repository.GetByIdAsync(id);
            if (gymClass == null)
                return GeneralResponse<ClassDto>.Failure("Class not found");

            if (request.StartTime >= request.EndTime)
            {
                return GeneralResponse<ClassDto>.Failure("Start time must be before end time");
            }

            if (request.MaxCapacity <= 0)
            {
                return GeneralResponse<ClassDto>.Failure("Max capacity must be greater than zero");
            }

            if (request.RoomId.HasValue)
            {
                var room = await _roomRepository.GetByIdAsync(request.RoomId.Value);
                if (room == null)
                {
                    return GeneralResponse<ClassDto>.Failure("Room not found");
                }

                var effectiveMaxCapacity = request.MaxCapacity > 0 ? request.MaxCapacity : gymClass.MaxCapacity;
                if (room.Capacity < effectiveMaxCapacity)
                {
                    return GeneralResponse<ClassDto>.Failure($"Room capacity ({room.Capacity}) is less than class max capacity ({effectiveMaxCapacity})");
                }

                var hasConflict = await HasRoomConflictAsync(request.RoomId.Value, request.StartTime, request.EndTime, id);
                if (hasConflict)
                {
                    return GeneralResponse<ClassDto>.Failure("Room is already booked for another class at this time");
                }
            }

            var effectiveTrainerId = request.TrainerId ?? gymClass.TrainerId;
            
            if (request.TrainerId.HasValue)
            {
                var trainer = await _userRepository.GetByIdAsync(request.TrainerId.Value);
                if (trainer == null)
                {
                    return GeneralResponse<ClassDto>.Failure("Trainer not found");
                }
            }

            var trainerConflict = await GetTrainerConflictAsync(effectiveTrainerId, request.StartTime, request.EndTime, id);
            if (trainerConflict != null)
            {
                return GeneralResponse<ClassDto>.FailureWithData($"Trainer is already scheduled for another class at this time: {trainerConflict.Title}", trainerConflict);
            }

            gymClass.Title = request.Title;
            gymClass.Description = request.Description;
            gymClass.Type = request.Type;
            gymClass.StartTime = request.StartTime;
            gymClass.EndTime = request.EndTime;
            gymClass.MaxCapacity = request.MaxCapacity;
            gymClass.Status = request.Status;

            if (request.TrainerId.HasValue)
                gymClass.TrainerId = request.TrainerId.Value;
            if (request.RoomId.HasValue)
                gymClass.RoomId = request.RoomId.Value;

            _repository.Update(gymClass);
            await _repository.SaveChangesAsync();

            return GeneralResponse<ClassDto>.Ok(new ClassDto
            {
                Id = gymClass.Id,
                Title = gymClass.Title,
                Description = gymClass.Description,
                Type = gymClass.Type,
                StartTime = gymClass.StartTime,
                EndTime = gymClass.EndTime,
                MaxCapacity = gymClass.MaxCapacity,
                CurrentBookingsCount = gymClass.CurrentBookingsCount,
                Status = gymClass.Status,
                RoomId = gymClass.RoomId,
                TrainerId = gymClass.TrainerId,
                TrainerName = string.Empty
            }, "Class updated successfully");
        }

        private async Task<bool> HasRoomConflictAsync(Guid roomId, DateTime startTime, DateTime endTime, Guid? excludeClassId = null)
        {
            var classes = await _repository.FindAsync(c => c.RoomId == roomId);
            
            var conflictingClasses = classes.Where(c => 
                c.StartTime < endTime && 
                c.EndTime > startTime);
            
            if (excludeClassId.HasValue)
            {
                conflictingClasses = conflictingClasses.Where(c => c.Id != excludeClassId.Value);
            }
            
            return conflictingClasses.Any();
        }

        private async Task<ClassDto?> GetTrainerConflictAsync(Guid trainerId, DateTime startTime, DateTime endTime, Guid? excludeClassId = null)
        {
            var classes = await _repository.FindAsync(c => c.TrainerId == trainerId);
            
            var conflictingClass = classes.FirstOrDefault(c => 
                c.StartTime < endTime && 
                c.EndTime > startTime &&
                (!excludeClassId.HasValue || c.Id != excludeClassId.Value));
            
            if (conflictingClass == null)
                return null;

            return new ClassDto
            {
                Id = conflictingClass.Id,
                Title = conflictingClass.Title,
                Description = conflictingClass.Description,
                Type = conflictingClass.Type,
                StartTime = conflictingClass.StartTime,
                EndTime = conflictingClass.EndTime,
                MaxCapacity = conflictingClass.MaxCapacity,
                CurrentBookingsCount = conflictingClass.CurrentBookingsCount,
                Status = conflictingClass.Status,
                RoomId = conflictingClass.RoomId,
                TrainerId = conflictingClass.TrainerId,
                TrainerName = string.Empty
            };
        }

        public async Task<GeneralResponse<bool>> DeleteClassAsync(Guid id)
        {
            var gymClass = await _repository.GetByIdAsync(id);
            if (gymClass == null)
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Data = false,
                    Message = "the class is not deleted"
                };
            }

            if (gymClass.CurrentBookingsCount > 0)
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Data = false,
                    Message = $"Cannot delete class with {gymClass.CurrentBookingsCount} existing bookings. Cancel bookings first."
                };
            }

            _repository.Remove(gymClass);
            await _repository.SaveChangesAsync();

            return new GeneralResponse<bool>
            {
                Success = true,
                Data = true,
                Message = "The item is deleted successfully"
            };
        }
    }
}
