using System;
using System.Collections.Generic;

namespace TicTacToe.Models;

public partial class Game
{
    public int GameId { get; set; }

    public string GameCode { get; set; } = null!;

    public string P1Name { get; set; } = null!;

    public string? P2Name { get; set; }

    public bool IsBeingPlayed { get; set; }

    public bool IsFinished { get; set; }

    public bool? IsP1Winner { get; set; }

    public virtual ICollection<GameClub> GameClubs { get; set; } = new List<GameClub>();

    public virtual ICollection<GameMove> GameMoves { get; set; } = new List<GameMove>();

    public virtual ICollection<Round> Rounds { get; set; } = new List<Round>();
}
