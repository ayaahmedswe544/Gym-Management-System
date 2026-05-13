using Gym_Management_System.Business.DTOs.ClassDTOs;
using Gym_Management_System.Business.DTOs.TrainerDTOs;
using Gym_Management_System.Business.GeneralResponse;
using Gym_Management_System.Business.IService;
using Gym_Management_System.Data.Models;
using Gym_Management_System.Data_Access.IRepository;

namespace Gym_Management_System.Business.Services
{
    public class TrainerService:ITrainerService
    {
        private readonly IRepository<TrainerProfile> _profileRepository;
        private readonly IRepository<GymClass> _classRepository;
        private readonly IRepository<TrainerAvailability> _availabilityRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IFileService _fileService;
        public TrainerService(
        IRepository<TrainerProfile> profileRepository,
        IRepository<GymClass> classRepository,
        IRepository<TrainerAvailability> availabilityRepository,
        IRepository<User> userRepository,
        IFileService fileService)
        {
            _profileRepository = profileRepository;
            _classRepository = classRepository;
            _availabilityRepository = availabilityRepository;
            _userRepository = userRepository;
            _fileService = fileService;
        }

        public async Task<GeneralResponse<List<TrainerProfileDto>>>GetAllTrainersAsync()
        {
            try
            {
                var profiles = await _profileRepository.GetAllAsync();
                if (profiles == null)
                {
                    return new GeneralResponse<List<TrainerProfileDto>>
                    {
                        Success = false,
                        Message = "There are no Trainers",
                        Data = null,
                        Errors = null
                    };
                }
                var dtos = new List<TrainerProfileDto>();
                foreach (var p in profiles)
                {
                    var user = await _userRepository.GetByIdAsync(p.UserId);
                    dtos.Add(new TrainerProfileDto
                    {
                        Id = p.Id,
                        UserId = p.UserId,
                        FullName = user?.FullName ?? string.Empty,
                        Bio = p.Bio,
                        Specialties = p.Specialties,
                        YearsOfExperience = p.YearsOfExperience,
                        SocialLinks = p.SocialLinks,
                        PhotoUrl=p.PhotoUrl
                    });
                }
                return new GeneralResponse<List<TrainerProfileDto>>
                {
                    Success = false,
                    Message = "Trainers Data pulled successfully",
                    Data = dtos,
                    Errors = null
                };
            }
            catch (Exception ex) {

                return new GeneralResponse<List<TrainerProfileDto>>
                {
                    Success = false,
                    Message = "Failed to get trainers data",
                    Data = null,
                    Errors = null
                };
            }

           
        }

        public async Task<GeneralResponse<TrainerDetailsDto>> GetTrainerAsync(Guid id)
        {
            var profiles = await _profileRepository.FindAsync(p => p.UserId == id || p.Id == id);
            var profile = profiles.FirstOrDefault();

            if (profile == null) return GeneralResponse<TrainerDetailsDto>.Failure("Trainer not found");

            var user = await _userRepository.GetByIdAsync(profile.UserId);

            var classes = await _classRepository.FindAsync(c => c.TrainerId == profile.UserId && c.StartTime > DateTime.UtcNow);
            var upcomingClasses = classes.OrderBy(c => c.StartTime).Select(c => new ClassDto
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                Type = c.Type,
                StartTime = c.StartTime,
                MaxCapacity = c.MaxCapacity,
                CurrentBookingsCount = c.CurrentBookingsCount,
                Status = c.Status
            }).ToList();

            var details = new TrainerDetailsDto
            {
                Profile = new TrainerProfileDto
                {
                    Id = profile.Id,
                    UserId = profile.UserId,
                    FullName = user?.FullName ?? string.Empty,
                    Bio = profile.Bio,
                    Specialties = profile.Specialties,
                    YearsOfExperience = profile.YearsOfExperience,
                    SocialLinks = profile.SocialLinks,
                    PhotoUrl=profile.PhotoUrl
                },
                UpcomingClasses = upcomingClasses
            };

            return GeneralResponse<TrainerDetailsDto>.Ok(details, "Trainer found.");
        }

        public async Task<GeneralResponse<TrainerProfileDto>> UpdateProfileAsync(UpdateTrainerProfileDto updatedProfileDto, Guid userId)
        {

            var profiles = await _profileRepository.FindAsync(p => p.UserId == userId);
            var profile = profiles.FirstOrDefault();

            if (profile == null)
            {
                return new GeneralResponse<TrainerProfileDto>
                {
                    Success = false,
                    Message="There is no profile for you",
                    Data=null,
                    Errors=null

                };
 
            }
            string? photoUrl = null;

            if (updatedProfileDto.Photo != null && updatedProfileDto.Photo.Length > 0)
            {
               
                if (profile != null && !string.IsNullOrEmpty(profile.PhotoUrl))
                {
                    _fileService.DeleteFile(profile.PhotoUrl);
                }

                photoUrl = await _fileService.UploadFileAsync(updatedProfileDto.Photo, "trainers");
            }

                profile.Bio = updatedProfileDto.Bio ?? profile.Bio;
                profile.Specialties = updatedProfileDto.Specialties??profile.Specialties;
                profile.YearsOfExperience = updatedProfileDto.YearsOfExperience??profile.YearsOfExperience;
                profile.SocialLinks = updatedProfileDto.SocialLinks?? profile.SocialLinks;
            if (photoUrl != null) { 
            profile.PhotoUrl= photoUrl;
            }

                _profileRepository.Update(profile);
            

            await _profileRepository.SaveChangesAsync();
            var user = await _userRepository.GetByIdAsync(userId);

            var dto = new TrainerProfileDto
            {
                Id = profile.Id,
                UserId = profile.UserId,
                FullName = user?.FullName ?? string.Empty,
                Bio = profile.Bio,
                Specialties = profile.Specialties,
                YearsOfExperience = profile.YearsOfExperience,
                SocialLinks = profile.SocialLinks,
                PhotoUrl=profile.PhotoUrl ?? string.Empty
            };

            return GeneralResponse<TrainerProfileDto>.Ok(dto, "Profile updated.");
        }

        public async Task<GeneralResponse<TrainerAvailabilityDto>> SetAvailabilityAsync(CreateTrainerAvailabilityDto availabilityDto, Guid trainerId)
        {
            var availability = new TrainerAvailability
            {
                TrainerId = trainerId,
                DayOfWeek = availabilityDto.DayOfWeek,
                StartTime = availabilityDto.StartTime,
                EndTime = availabilityDto.EndTime
            };
            await _availabilityRepository.AddAsync(availability);
            await _availabilityRepository.SaveChangesAsync();

            var dto = new TrainerAvailabilityDto
            {
                Id = availability.Id,
                TrainerId = availability.TrainerId,
                DayOfWeek = availability.DayOfWeek,
                StartTime = availability.StartTime,
                EndTime = availability.EndTime
            };

            return GeneralResponse<TrainerAvailabilityDto>.Ok(dto, "Availability saved.");
        }
    }
}
