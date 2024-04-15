using Microsoft.AspNetCore.SignalR;

namespace TimeTrackingSystem.Hubs
{
    public static class UserList
    {
        public static Dictionary<string, string> List = new Dictionary<string, string>();
    }

    public class MeetingHubServer : Hub
    {
        public async Task JoinRoom(string roomId, string userId)
        {
            UserList.List.Add(Context.ConnectionId, userId);
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
            await Clients.OthersInGroup(roomId).SendAsync("UserConnnected", userId);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (UserList.List.ContainsKey(Context.ConnectionId))
            {
                await Clients.All.SendAsync("UserDisconnected", UserList.List[Context.ConnectionId]);
            }
        }

        public async Task StopShareScreen(string peerID, string streamID)
        {
            await Clients.Others.SendAsync("StopShare", peerID, streamID);
        }
    }
}
