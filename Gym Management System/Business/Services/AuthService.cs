using Gym_Management_System.Business.DTOs.AuthDTOs;
using Gym_Management_System.Business.GeneralResponse;
using Gym_Management_System.Business.IService;
using Gym_Management_System.Data.Models;
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

        public AuthService(
            UserManager<User> userManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            SignInManager<User> signInManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _configuration = configuration;
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

            return GeneralResponse<AuthResponse>.Ok(new AuthResponse { Email = user.Email, Token = string.Empty }, "User registered successfully");
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

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var token = GetToken(authClaims);

                return GeneralResponse<AuthResponse>.Ok(new AuthResponse
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    Expiration = token.ValidTo,
                    Email = user.Email!
                }, "Login successful");
            }
            return GeneralResponse<AuthResponse>.Failure("Invalid credentials");
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
