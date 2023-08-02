using System;
using System.Collections.Generic;

namespace TicTacToe.Models;

public partial class PlayerClubHistory
{
    public int PlayerClubHistoryId { get; set; }

    public int? PlayerId { get; set; }

    public int? ClubId { get; set; }

    public virtual Club? Club { get; set; }

    public virtual Player? Player { get; set; }
}
