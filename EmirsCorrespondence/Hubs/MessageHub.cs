using Microsoft.AspNetCore.SignalR;

namespace EmirsCorrespondence.Hubs
{
    public class MessageHub : Hub
    {
        public async Task SendMessageNotification(string userId, string message)
        {
            await Clients.User(userId).SendAsync("ReceiveMessageNotification", message);
        }
    }
}
