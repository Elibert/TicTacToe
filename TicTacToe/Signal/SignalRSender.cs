using System.Net;
using System.Xml.Linq;
using Microsoft.AspNetCore.SignalR.Client;
using TicTacToe.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace TicTacToe.Signal
{
    public class SignalRSender
    {
        private IConfiguration _config;
        private HubConnection connection;
        public SignalRSender(IConfiguration config)
        {
            _config = config;
            connection = new HubConnectionBuilder()
                .WithUrl(_config.GetSection("Settings:url").Value + "TicTacToeHub")
                .Build();
        }
        public void EnterGame(int gameId)
        {
            connection.StartAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    //Do something if the connection failed
                }
                else
                {
                    //if connection is successfull, do something
                    connection.InvokeAsync("StartGame", gameId);

                }
            }).Wait();
        }

        public void MakeMove(int userId, int? coordinateX, int? coordinateY, TicTacToeTypes? moveType, bool isRoundFinished, bool isP1turn, List<int> combination)
        {
            connection.StartAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    //Do something if the connection failed
                }
                else
                {
                    //if connection is successfull, do something
                    connection.InvokeAsync("MakeMove",userId, coordinateX, coordinateY, moveType, isRoundFinished, isP1turn, combination);

                }
            }).Wait();
        }


        public void SelectedPlayer(int userId,string playerName)
        {
            connection.StartAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    //Do something if the connection failed
                }
                else
                {
                    //if connection is successfull, do something
                    connection.InvokeAsync("SelectedPlayer", userId, playerName);
                }
            }).Wait();
        }

        public void ChangeRoundClubs(int P1UserId, int P2UserId, Dictionary<string, string> newRoundClubs,bool isP1turn, int P1rounds, int P2rounds)
        {
            connection.StartAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    //Do something if the connection failed
                }
                else
                {
                    //if connection is successfull, do something
                    connection.InvokeAsync("ChangeClubs", P1UserId, P2UserId, newRoundClubs, isP1turn, P1rounds, P2rounds);
                }
            }).Wait();
        }
    }
}
