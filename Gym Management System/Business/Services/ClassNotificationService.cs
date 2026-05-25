using Gym_Management_System.Business.Hubs;
using Gym_Management_System.Business.IService;
using Microsoft.AspNetCore.SignalR;

namespace Gym_Management_System.Business.Services
{
    public class ClassNotificationService:IClassNotificationService
    {
        private readonly IHubContext<ClassHub> _hubContext;

        public ClassNotificationService(IHubContext<ClassHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyClassUpdate(Guid classId, int maxCapacity, int currentBookings)
        {
            var remainingSlots = maxCapacity - currentBookings;

            var status = remainingSlots switch
            {
                0 => "Full",
                <= 3 => "AlmostFull",
                _ => "Available"
            };

            var groupName = $"class_{classId}";

            await _hubContext.Clients.Group(groupName)
                .SendAsync("ClassUpdated", new
                {
                    classId,
                    remainingSlots,
                    status
                });
            if (remainingSlots == 0)
            {
                await _hubContext.Clients.Group(groupName)
                    .SendAsync("ClassFull", new
                    {
                        classId,
                        message = "Last seat taken!"
                    });
            }
            else if (remainingSlots <= 3)
            {
                await _hubContext.Clients.Group(groupName)
                    .SendAsync("ClassAlmostFull", new
                    {
                        classId,
                        remainingSlots
                    });
            }
        }
    }
}
