namespace Gym_Management_System.Business.IService
{
    public interface IClassNotificationService
    {
        Task NotifyClassUpdate(Guid classId, int maxCapacity, int currentBookings);
    }
}
