using Gym_Management_System.Business.DTOs.AuthDTOs;
using Gym_Management_System.Business.GeneralResponse;
using Gym_Management_System.Business.IService;
using Gym_Management_System.Data.Models;
using Gym_Management_System.Data_Access.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Gym_Management_System.Business.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IRepository<TrainerProfile> _trainerProfileRepository;
        private readonly IRepository<User> _userRepository;

        public AuthService(
            UserManager<User> userManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            SignInManager<User> signInManager,
            IConfiguration configuration,
            IRepository<TrainerProfile> trainerProfileRepository,
            IRepository<User> userRepository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _trainerProfileRepository = trainerProfileRepository;
            _userRepository = userRepository;
        }

        public async Task<GeneralResponse<AuthResponse>> RegisterAsync(RegisterRequest request, string role)
        {
            var userExists = await _userManager.FindByEmailAsync(request.Email);
            if (userExists != null)
                return GeneralResponse<AuthResponse>.Failure("User already exists!");

            User user = new()
            {
                Email = request.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = request.Email,
                FullName = request.FullName,
                PhoneNumber = request.PhoneNumber,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
                return GeneralResponse<AuthResponse>.Failure("User creation failed! Please check user details and try again.");

            string[] roleNames = { "Admin", "Trainer", "Member" };
            foreach (var roleName in roleNames)
            {
                var roleExist = await _roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await _roleManager.CreateAsync(new IdentityRole<Guid> { Name = roleName });
                }
            }

            if (roleNames.Contains(role))
            {
                await _userManager.AddToRoleAsync(user, role);
            }
            else
            {
                await _userManager.AddToRoleAsync(user, "Member");
            }

            if (role == "Trainer")
            {
                var trainerProfile = new TrainerProfile
                {
                    UserId = user.Id,
                    Bio = string.Empty,
                    Specialties = string.Empty,
                    YearsOfExperience = 0,
                    SocialLinks = string.Empty,
                    PhotoUrl = string.Empty
                };
                await _trainerProfileRepository.AddAsync(trainerProfile);
                await _trainerProfileRepository.SaveChangesAsync();
            }
            return GeneralResponse<AuthResponse>.Ok(new AuthResponse { Email = user.Email, PhoneNumber = user.PhoneNumber ?? string.Empty, Token = string.Empty }, "User registered successfully");
        }

        public async Task<GeneralResponse<AuthResponse>> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, request.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email!),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

                if (!string.IsNullOrEmpty(user.PhoneNumber))
                {
                    authClaims.Add(new Claim(ClaimTypes.MobilePhone, user.PhoneNumber));
                }

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var token = GetToken(authClaims);

                return GeneralResponse<AuthResponse>.Ok(new AuthResponse
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    Expiration = token.ValidTo,
                    Email = user.Email!,
                    PhoneNumber = user.PhoneNumber ?? string.Empty
                }, "Login successful");
            }
            return GeneralResponse<AuthResponse>.Failure("Invalid credentials");
        }

        public async Task<GeneralResponse<ProfileDto>> GetProfileAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return GeneralResponse<ProfileDto>.Failure("User not found");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var primaryRole = roles.FirstOrDefault() ?? "Member";
            var isTrainer = roles.Contains("Trainer");

            TrainerProfileInfo? trainerProfileInfo = null;
            if (isTrainer)
            {
                var trainerProfile = await _trainerProfileRepository.GetByIdAsync(userId);
                if (trainerProfile != null)
                {
                    trainerProfileInfo = new TrainerProfileInfo
                    {
                        Bio = trainerProfile.Bio ?? string.Empty,
                        Specialties = trainerProfile.Specialties ?? string.Empty,
                        YearsOfExperience = trainerProfile.YearsOfExperience,
                        SocialLinks = trainerProfile.SocialLinks ?? string.Empty,
                        PhotoUrl = trainerProfile.PhotoUrl ?? string.Empty
                    };
                }
            }

            var profile = new ProfileDto
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty,
                FullName = user.FullName ?? string.Empty,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                CreatedAt = user.CreatedAt,
                Role = primaryRole,
                IsTrainer = isTrainer,
                TrainerProfile = trainerProfileInfo
            };

            return GeneralResponse<ProfileDto>.Ok(profile, "Profile retrieved successfully");
        }

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "supersecretkey"));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return token;
        }
    }
}
