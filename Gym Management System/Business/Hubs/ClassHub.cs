using Microsoft.AspNetCore.SignalR;

namespace Gym_Management_System.Business.Hubs
{
    public class ClassHub:Hub
    {
        public async Task JoinClassRoom(string classId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"class_{classId}");
        }

        public async Task LeaveClassRoom(string classId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"class_{classId}");
        }
    }
}
