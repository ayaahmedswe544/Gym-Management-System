namespace Gym_Management_System.Business.DTOs.AuthDTOs
{
    public class AuthResponse
    {
        public string Token { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
    }
}
