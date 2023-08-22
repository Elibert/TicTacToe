using System;
using System.Collections.Generic;

namespace TicTacToe.Models;

public partial class Club
{
    public int ClubId { get; set; }

    public string? ClubName { get; set; }

    public string? ClubLogo { get; set; }

    public string? ApiTeamId { get; set; }

    public virtual ICollection<PlayerClubHistory> PlayerClubHistories { get; set; } = new List<PlayerClubHistory>();

    public virtual ICollection<RoundClub> RoundClubs { get; set; } = new List<RoundClub>();
}
