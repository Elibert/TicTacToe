namespace TicTacToe.Models
{
    public class GamePlay
    {
        public int GameId { get; set; }
        public int CoordinateX { get; set; }
        
        public int CoordinateY { get; set; }

        public int PlayerId { get; set; }

        public TicTacToeTypes Movetype { get; set; }
    }
}
