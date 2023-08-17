using System;
using System.Collections.Generic;

namespace TicTacToe.Models;

public partial class Player
{
    public int PlayerId { get; set; }

    public string? PlayerName { get; set; }

    public DateTime? Birthdate { get; set; }

    public string? ApiPlayerId { get; set; }

    public virtual ICollection<PlayerClubHistory> PlayerClubHistories { get; set; } = new List<PlayerClubHistory>();
}
