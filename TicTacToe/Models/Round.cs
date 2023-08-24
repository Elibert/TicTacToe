using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicTacToe.Models;

public partial class Round
{
    [NotMapped]
    public bool isPlayerTurn { get; set; }
}
