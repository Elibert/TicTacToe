using System;
using System.Collections.Generic;

namespace TicTacToe.Models;

public partial class Club
{
    public int ClubId { get; set; }

    public string? ClubName { get; set; }

    public string? ClubLogo { get; set; }

    public virtual ICollection<PlayerClubHistory> PlayerClubHistories { get; set; } = new List<PlayerClubHistory>();
}
