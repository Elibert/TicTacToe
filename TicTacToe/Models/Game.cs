using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicTacToe.Models;

public partial class Game
{
    public static Game createGame()
    {
        Game game = new Game();
        game.IsBeingPlayed = false;
        game.IsFinished = false;
        return game;
    }

    [NotMapped]
    public TicTacToeTypes MoveType { get; set; }

    [NotMapped]
    public int OpponentUserId { get; set; }
}
