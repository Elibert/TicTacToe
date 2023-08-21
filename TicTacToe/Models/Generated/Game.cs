using System;
using System.Collections.Generic;

namespace TicTacToe.Models;

public partial class Game
{
    public int GameId { get; set; }

    public string GameCode { get; set; } = null!;

    public int P1UserId { get; set; }

    public int? P2UserId { get; set; }

    public bool IsBeingPlayed { get; set; }

    public bool IsFinished { get; set; }

    public bool? IsP1Winner { get; set; }

    public virtual User P1User { get; set; } = null!;

    public virtual User? P2User { get; set; }

    public virtual ICollection<Round> Rounds { get; set; } = new List<Round>();
}
