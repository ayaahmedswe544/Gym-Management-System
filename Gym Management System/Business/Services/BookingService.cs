using Gym_Management_System.Business.DTOs.BookingDTOs;
using Gym_Management_System.Business.GeneralResponse;
using Gym_Management_System.Business.IService;
using Gym_Management_System.Data;
using Gym_Management_System.Data.Enums;
using Gym_Management_System.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Gym_Management_System.Business.Services
{
    public class BookingService : IBookingService
    {
        private readonly ApplicationDbContext _db;
        private readonly IClassNotificationService _notificationService;

        public BookingService(ApplicationDbContext db, IClassNotificationService notificationService)
        {
            _db = db;
            _notificationService = notificationService;
        }

        public async Task<GeneralResponse<BookingDto>> CreateBookingAsync(CreateBookingDto request, Guid userId)
        {
            using var transaction = await _db.Database.BeginTransactionAsync(IsolationLevel.Serializable);

            try
            {
                var gymClass = await _db.GymClasses
                    .FirstOrDefaultAsync(c => c.Id == request.GymClassId);

                if (gymClass == null)
                    return GeneralResponse<BookingDto>.Failure("Class not found");

                if (gymClass.CurrentBookingsCount >= gymClass.MaxCapacity)
                    return GeneralResponse<BookingDto>.Failure("Class is full");

                if (gymClass.StartTime <= DateTime.UtcNow)
                    return GeneralResponse<BookingDto>.Failure("Cannot book a class that has already started");

                var alreadyBooked = await _db.Bookings
                    .AnyAsync(b => b.UserId == userId && b.GymClassId == request.GymClassId && b.Status == BookingStatus.Confirmed);

                if (alreadyBooked)
                    return GeneralResponse<BookingDto>.Failure("You already have a confirmed booking for this class");

                gymClass.CurrentBookingsCount++;

                var booking = new Booking
                {
                    UserId = userId,
                    GymClassId = request.GymClassId,
                    Status = BookingStatus.Confirmed,
                    CreatedAt = DateTime.UtcNow
                };

                await _db.Bookings.AddAsync(booking);
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
                await _notificationService.NotifyClassUpdate(gymClass.Id, gymClass.MaxCapacity, gymClass.CurrentBookingsCount);

                return GeneralResponse<BookingDto>.Ok(new BookingDto
                {
                    Id = booking.Id,
                    UserId = booking.UserId,
                    GymClassId = booking.GymClassId,
                    Status = booking.Status,
                    CreatedAt = booking.CreatedAt
                }, "Booking confirmed");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return GeneralResponse<BookingDto>.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<GeneralResponse<string>> CancelBookingAsync(Guid bookingId, Guid userId)
        {
            using var transaction = await _db.Database.BeginTransactionAsync(IsolationLevel.Serializable);

            try
            {
                var booking = await _db.Bookings
                    .Include(b => b.GymClass)
                    .FirstOrDefaultAsync(b => b.Id == bookingId && b.UserId == userId);

                if (booking == null)
                    return GeneralResponse<string>.Failure("Booking not found");

                if (booking.Status == BookingStatus.Cancelled)
                    return GeneralResponse<string>.Failure("Booking is already cancelled");

                booking.Status = BookingStatus.Cancelled;
                booking.CancelledAt = DateTime.UtcNow;

                if (booking.GymClass != null)
                {
                    booking.GymClass.CurrentBookingsCount--;
                }

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                if (booking.GymClass != null)
                {
                    await _notificationService.NotifyClassUpdate(booking.GymClass.Id, booking.GymClass.MaxCapacity, booking.GymClass.CurrentBookingsCount);
                }

                return GeneralResponse<string>.Ok("Booking cancelled successfully", "Booking cancelled successfully");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return GeneralResponse<string>.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<GeneralResponse<IEnumerable<BookingDto>>> GetMyBookingsAsync(Guid userId)
        {
            var bookings = await _db.Bookings
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            var dtos = bookings.Select(b => new BookingDto
            {
                Id = b.Id,
                UserId = b.UserId,
                GymClassId = b.GymClassId,
                Status = b.Status,
                CreatedAt = b.CreatedAt
            });

            return GeneralResponse<IEnumerable<BookingDto>>.Ok(dtos);
        }

        public async Task<GeneralResponse<BookingDto>> GetBookingByIdAsync(Guid bookingId)
        {
            var booking = await _db.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId);
            if (booking == null)
                return GeneralResponse<BookingDto>.Failure("Booking not found");

            return GeneralResponse<BookingDto>.Ok(new BookingDto
            {
                Id = booking.Id,
                UserId = booking.UserId,
                GymClassId = booking.GymClassId,
                Status = booking.Status,
                CreatedAt = booking.CreatedAt
            });
        }

        public async Task<GeneralResponse<IEnumerable<BookingDto>>> GetBookingsByUserIdAsync(Guid userId)
        {
            var bookings = await _db.Bookings
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            var dtos = bookings.Select(b => new BookingDto
            {
                Id = b.Id,
                UserId = b.UserId,
                GymClassId = b.GymClassId,
                Status = b.Status,
                CreatedAt = b.CreatedAt
            });

            return GeneralResponse<IEnumerable<BookingDto>>.Ok(dtos);
        }

        public async Task<GeneralResponse<BookingDto>> UpdateBookingAsync(Guid bookingId, UpdateBookingDto request)
        {
            using var transaction = await _db.Database.BeginTransactionAsync(IsolationLevel.Serializable);

            try
            {
                var booking = await _db.Bookings
                    .Include(b => b.GymClass)
                    .FirstOrDefaultAsync(b => b.Id == bookingId);

                if (booking == null)
                    return GeneralResponse<BookingDto>.Failure("Booking not found");

                var oldClassId = booking.GymClassId;
                var newClassId = request.GymClassId;
                var oldStatus = booking.Status;
                var newStatus = request.Status;

                if (oldClassId != newClassId)
                {
                    var newClass = await _db.GymClasses.FirstOrDefaultAsync(c => c.Id == newClassId);
                    if (newClass == null)
                        return GeneralResponse<BookingDto>.Failure("New gym class not found");

                    if (oldStatus == BookingStatus.Confirmed)
                    {
                        if (booking.GymClass != null)
                        {
                            booking.GymClass.CurrentBookingsCount = Math.Max(0, booking.GymClass.CurrentBookingsCount - 1);
                        }
                    }

                    if (newStatus == BookingStatus.Confirmed)
                    {
                        if (newClass.CurrentBookingsCount >= newClass.MaxCapacity)
                            return GeneralResponse<BookingDto>.Failure("New gym class is full");
                        newClass.CurrentBookingsCount++;
                    }

                    booking.GymClassId = newClassId;
                }
                else
                {
                    if (oldStatus != newStatus)
                    {
                        if (booking.GymClass != null)
                        {
                            if (oldStatus == BookingStatus.Confirmed && newStatus != BookingStatus.Confirmed)
                            {
                                booking.GymClass.CurrentBookingsCount = Math.Max(0, booking.GymClass.CurrentBookingsCount - 1);
                            }
                            else if (oldStatus != BookingStatus.Confirmed && newStatus == BookingStatus.Confirmed)
                            {
                                if (booking.GymClass.CurrentBookingsCount >= booking.GymClass.MaxCapacity)
                                    return GeneralResponse<BookingDto>.Failure("Gym class is full");
                                booking.GymClass.CurrentBookingsCount++;
                            }
                        }
                    }
                }

                booking.Status = newStatus;
                if (newStatus == BookingStatus.Cancelled)
                {
                    booking.CancelledAt = DateTime.UtcNow;
                }
                else
                {
                    booking.CancelledAt = null;
                }

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                if (oldClassId != newClassId)
                {
                    if (booking.GymClass != null)
                    {
                        await _notificationService.NotifyClassUpdate(booking.GymClass.Id, booking.GymClass.MaxCapacity, booking.GymClass.CurrentBookingsCount);
                    }
                    var newClass = await _db.GymClasses.FirstOrDefaultAsync(c => c.Id == newClassId);
                    if (newClass != null)
                    {
                        await _notificationService.NotifyClassUpdate(newClass.Id, newClass.MaxCapacity, newClass.CurrentBookingsCount);
                    }
                }
                else if (booking.GymClass != null)
                {
                    await _notificationService.NotifyClassUpdate(booking.GymClass.Id, booking.GymClass.MaxCapacity, booking.GymClass.CurrentBookingsCount);
                }

                return GeneralResponse<BookingDto>.Ok(new BookingDto
                {
                    Id = booking.Id,
                    UserId = booking.UserId,
                    GymClassId = booking.GymClassId,
                    Status = booking.Status,
                    CreatedAt = booking.CreatedAt
                }, "Booking updated successfully");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return GeneralResponse<BookingDto>.Failure($"An error occurred: {ex.Message}");
            }
        }
    }
}