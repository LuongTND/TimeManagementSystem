using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;
using System.Security.Claims;

namespace ShoppingWebsite.Hubs
{
    public class PaymentHubServer : Hub
    {
        public async Task JoinRoom()
        {
            if (!Context.User.Identity.IsAuthenticated) return;
            await Groups.AddToGroupAsync(Context.ConnectionId, Context.User.FindFirstValue("Id"));
        }
    }
}
