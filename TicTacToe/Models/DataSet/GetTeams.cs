namespace TicTacToe.Models.DataSet
{
    public class GetTeams
    {
        public Api api { get; set; }
    }
    public class Api
    {
        public int results { get; set; }
        public List<Team> teams { get; set; }
    }
}
