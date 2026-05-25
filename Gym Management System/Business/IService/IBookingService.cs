using Gym_Management_System.Business.DTOs.BookingDTOs;
using Gym_Management_System.Business.GeneralResponse;

namespace Gym_Management_System.Business.IService
{
    public interface IBookingService
    {
        Task<GeneralResponse<BookingDto>> CreateBookingAsync(CreateBookingDto request, Guid userId);
        Task<GeneralResponse<string>> CancelBookingAsync(Guid bookingId, Guid userId);
        Task<GeneralResponse<IEnumerable<BookingDto>>> GetMyBookingsAsync(Guid userId);
        Task<GeneralResponse<BookingDto>> UpdateBookingAsync(Guid bookingId, UpdateBookingDto request);
        Task<GeneralResponse<BookingDto>> GetBookingByIdAsync(Guid bookingId);
        Task<GeneralResponse<IEnumerable<BookingDto>>> GetBookingsByUserIdAsync(Guid userId);
    }
}
