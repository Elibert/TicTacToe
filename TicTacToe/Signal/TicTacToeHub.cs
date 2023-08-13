using Microsoft.AspNetCore.SignalR;
using TicTacToe.Data;
using TicTacToe.Models;

namespace TicTacToe.Signal
{
    public class TicTacToeHub : Hub
    {
        private static Dictionary<string, HashSet<string>> _userConnections = new Dictionary<string, HashSet<string>>();

        private readonly TictactoeContext context;

        public TicTacToeHub(TictactoeContext tictactoeContext)
        {
            context = tictactoeContext;
        }
        //public override async Task OnConnectedAsync()
        //{
        //    string userIdentifier = Context.GetHttpContext().Request.Query["userId"];

        //    if (!string.IsNullOrEmpty(userIdentifier))
        //    {
        //        if (!_userConnections.ContainsKey(userIdentifier))
        //        {
        //            _userConnections[userIdentifier] = new HashSet<string>();
        //        }
        //        _userConnections[userIdentifier].Add(Context.ConnectionId);
        //    }

        //    await base.OnConnectedAsync();
        //}

        //public override async Task OnDisconnectedAsync(Exception exception)
        //{
        //    //var connections = context.UserConnections.Where(c => c.ConnectionId == Context.ConnectionId);
        //    //if (connections.Count()>0)
        //    //{
        //    //    context.UserConnections.RemoveRange(connections);
        //    //    context.SaveChanges();
        //    //}
        //    //await base.OnDisconnectedAsync(exception);
        //}

        public void Subscribe(string user)
        {
            if (!string.IsNullOrEmpty(user))
            {
                if (context.Users.Where(u=>u.UserName==user).Count()>0)
                {
                    int userId = context.Users.Where(u => u.UserName == user).OrderByDescending(p => p.UserId).Last().UserId;
                    context.UserConnections.Add(new Models.UserConnection { UserId = userId, ConnectionId = Context.ConnectionId });
                    context.SaveChanges();
                }
            }
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
        public async Task StartGame(int gameId)
        {
            int userId = context.Games.Where(g => g.GameId == gameId).First().P1UserId;
           await Clients.Client(GetConnectionId(userId)).SendAsync("ChangeScreenEnterGame", gameId);
        }

        public async Task MakeMove(int userId, int coordinateX, int coordinateY, TicTacToeTypes? moveType, bool isRoundfinished)
        {
            await Clients.Client(GetConnectionId(userId)).SendAsync("changeTurns", coordinateX, coordinateY, moveType, isRoundfinished);
        }
        public async Task SelectedPlayer(int userId, string playerName)
        {
            await Clients.Client(GetConnectionId(userId)).SendAsync("selectedPlayer", playerName);
        }

        public string GetConnectionId(int userId)
        {
            return context.UserConnections.Where(u => u.UserId == userId).OrderByDescending(o => o.UserConnectionId).Last().ConnectionId;
        }
    }
}
