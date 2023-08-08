using System;
using System.Collections.Generic;

namespace TicTacToe.Models;

public partial class Round
{
    public int RoundId { get; set; }

    public int RoundNo { get; set; }

    public int GameId { get; set; }

    public bool? IsP1Win { get; set; }

    public bool? IsFinished { get; set; }

    public virtual Game Game { get; set; } = null!;

    public virtual ICollection<GameMove> GameMoves { get; set; } = new List<GameMove>();
}
