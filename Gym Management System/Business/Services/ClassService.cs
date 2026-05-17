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

        public ClassService(IRepository<GymClass> repository)
        {
            _repository = repository;
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
                RoomId= request.RoomId

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
                MaxCapacity = gymClass.MaxCapacity,
                CurrentBookingsCount = gymClass.CurrentBookingsCount,
                Status = gymClass.Status,
                RoomId = gymClass.RoomId
            }, "Class created successfully");
        }
        public async Task<GeneralResponse<ClassDto>> UpdateClassAsync(Guid id, UpdateClassDto request)
        {
            var gymClass = await _repository.GetByIdAsync(id);
            if (gymClass == null)
                return GeneralResponse<ClassDto>.Failure("Class not found");

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
                MaxCapacity = gymClass.MaxCapacity,
                CurrentBookingsCount = gymClass.CurrentBookingsCount,
                Status = gymClass.Status,
                RoomId= gymClass.RoomId
            }, "Class updated successfully");
        }

        public async Task<GeneralResponse<bool>> DeleteClassAsync(Guid id)
        {
            var gymClass = await _repository.GetByIdAsync(id);
            if (gymClass == null)
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Data=false,
                    Message = "the class is not deleted"

                };
            }
                

            _repository.Remove(gymClass);
            await _repository.SaveChangesAsync();

            return new GeneralResponse<bool>
            {
                Success = true,
                Data=true,
                Message = "Tthe items is deleted successfuly"

            };
        }
    }
}
