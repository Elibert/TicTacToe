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

    [NotMapped]
    public Round CurrentRound { get{ return Rounds.OrderByDescending(r => r.RoundNo).First(); }}

    [NotMapped]
    public List<RoundClub> CurrentRoundClubs { get { return Rounds.OrderByDescending(r => r.RoundNo).First().RoundClubs.ToList(); } }
    
    [NotMapped]
    public List<RoundMove> CurrentRoundMoves { get { return Rounds.OrderByDescending(r => r.RoundNo).First().RoundMoves.ToList(); } }

    [NotMapped]
    public int RoundsWonByFirstPlayer { get { return Rounds.Where(r => (bool)r.IsP1Win && (bool)r.IsFinished).Count(); } set { } }

    [NotMapped]
    public int RoundsWonBySecondPlayer { get { return Rounds.Where(r => (bool)!r.IsP1Win && (bool)r.IsFinished).Count(); } set { } }

}
