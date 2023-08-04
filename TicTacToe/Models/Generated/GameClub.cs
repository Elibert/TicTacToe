using System;
using System.Collections.Generic;

namespace TicTacToe.Models;

public partial class GameClub
{
    public int GameClubId { get; set; }

    public int GameId { get; set; }

    public int ClubId { get; set; }

    public int RowNo { get; set; }

    public int ColNo { get; set; }

    public virtual Game Game { get; set; } = null!;
}
