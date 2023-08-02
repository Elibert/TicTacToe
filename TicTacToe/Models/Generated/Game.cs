using System;
using System.Collections.Generic;

namespace TicTacToe.Models;

public partial class Game
{
    public int GameId { get; set; }

    public string GameCode { get; set; } = null!;

    public string P1Name { get; set; } = null!;

    public string P2Name { get; set; } = null!;

    public bool? IsBeingPlayed { get; set; }

    public bool? IsFinished { get; set; }

    public virtual ICollection<Round> Rounds { get; set; } = new List<Round>();
}
