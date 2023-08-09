using System;
using System.Collections.Generic;

namespace TicTacToe.Models;

public partial class UserConnection
{
    public int UserConnectionId { get; set; }

    public int UserId { get; set; }

    public string ConnectionId { get; set; } = null!;
}
