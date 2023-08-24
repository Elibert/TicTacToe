using System;
using System.Collections.Generic;

namespace TicTacToe.Models;

public partial class RoundMove
{
    public int MoveId { get; set; }

    public int RoundId { get; set; }

    public int RowNo { get; set; }

    public int ColNo { get; set; }

    public string CellValue { get; set; } = null!;

    public virtual Round Round { get; set; } = null!;
}
