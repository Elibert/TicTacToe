using System;
using System.Collections.Generic;

namespace TicTacToe.Models;

public partial class Club
{
    public int ClubId { get; set; }

    public string? ClubName { get; set; }

    public string? ClubLogo { get; set; }

    public string? ApiTeamId { get; set; }

    public virtual ICollection<GameClub> GameClubs { get; set; } = new List<GameClub>();

    public virtual ICollection<PlayerClubHistory> PlayerClubHistories { get; set; } = new List<PlayerClubHistory>();
}
