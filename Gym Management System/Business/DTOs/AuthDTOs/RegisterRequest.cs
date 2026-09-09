using System.ComponentModel.DataAnnotations;

namespace Gym_Management_System.Business.DTOs.AuthDTOs
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Full Name is required.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^[0-9]{11}$", ErrorMessage = "Phone number must consist of exactly 11 digits (e.g., 01012345678).")]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}

