using System;
using System.Collections.Generic;

namespace TicTacToe.Models;

public partial class GameMove
{
    public int MoveId { get; set; }

    public int RoundId { get; set; }

    public int RowNo { get; set; }

    public int ColNo { get; set; }

    public int CellValue { get; set; }

    public virtual Round Round { get; set; } = null!;
}
