using Gym_Management_System.Business.DTOs.AuthDTOs;
using Gym_Management_System.Business.GeneralResponse;
using Microsoft.AspNetCore.Identity.Data;
using LoginRequest = Gym_Management_System.Business.DTOs.AuthDTOs.LoginRequest;
using RegisterRequest = Gym_Management_System.Business.DTOs.AuthDTOs.RegisterRequest;

namespace Gym_Management_System.Business.IService
{
    public interface IAuthService
    {
        Task<GeneralResponse<AuthResponse>> RegisterAsync(RegisterRequest request, string role);
        Task<GeneralResponse<AuthResponse>> LoginAsync(LoginRequest request);
    }
}
