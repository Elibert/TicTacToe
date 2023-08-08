using Microsoft.AspNetCore.SignalR;

namespace TicTacToe.Signal
{
    public class TicTacToeHub : Hub
    {
        private static Dictionary<string, HashSet<string>> _userConnections = new Dictionary<string, HashSet<string>>();

        public override async Task OnConnectedAsync()
        {
            string userIdentifier = Context.GetHttpContext().Request.Query["userId"];

            if (!string.IsNullOrEmpty(userIdentifier))
            {
                if (!_userConnections.ContainsKey(userIdentifier))
                {
                    _userConnections[userIdentifier] = new HashSet<string>();
                }
                _userConnections[userIdentifier].Add(Context.ConnectionId);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            string userIdentifier = Context.GetHttpContext().Request.Query["userId"];

            if (!string.IsNullOrEmpty(userIdentifier) && _userConnections.ContainsKey(userIdentifier))
            {
                _userConnections[userIdentifier].Remove(Context.ConnectionId);
                if (_userConnections[userIdentifier].Count == 0)
                {
                    _userConnections.Remove(userIdentifier);
                }
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessageToUser(string userId, string message)
        {
            if (_userConnections.TryGetValue(userId, out var connectionIds))
            {
                foreach (var connectionId in connectionIds)
                {
                    await Clients.Client(connectionId).SendAsync("ReceiveMessage", message);
                }
            }
            else
            {
                // User not found or not connected
                // Handle accordingly
            }
        }
    }
}
