using System;
using System.Collections.Generic;

namespace TicTacToe.Models;

public partial class User
{
    public int UserId { get; set; }

    public string UserName { get; set; } = null!;

    public virtual ICollection<Game> GameP1Users { get; set; } = new List<Game>();

    public virtual ICollection<Game> GameP2Users { get; set; } = new List<Game>();
}
