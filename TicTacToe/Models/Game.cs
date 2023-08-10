using System;
using System.Collections.Generic;

namespace TicTacToe.Models;

public partial class Game
{
    public static Game createGame()
    {
        Game game = new Game();
        game.IsBeingPlayed = false;
        game.IsFinished = false;
        game.GameCode = Guid.NewGuid().ToString();
        return game;
    }

    public TicTacToeTypes MoveType { get; set; }
}
