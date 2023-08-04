using System;
using System.Collections.Generic;

namespace TicTacToe.Models;

public partial class Game
{
    public static Game createGame(string playername)
    {
        Game game = new Game();
        game.P1Name = playername;
        game.IsBeingPlayed = false;
        game.IsFinished = false;
        game.GameCode = Guid.NewGuid().ToString();
        return game;
    }
}
