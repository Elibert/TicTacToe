using System;
using System.Collections.Generic;

namespace TicTacToe.Models;

public partial class RoundClub
{
    public int GameClubId { get; set; }

    public int RoundId { get; set; }

    public int ClubId { get; set; }

    public int RowNo { get; set; }

    public int ColNo { get; set; }

    public virtual Club Club { get; set; } = null!;

    public virtual Round Round { get; set; } = null!;
}
