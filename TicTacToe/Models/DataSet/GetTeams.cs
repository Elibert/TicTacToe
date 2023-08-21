namespace TicTacToe.Models.DataSet
{
    public class GetTeams
    {
        public string id { get; set; }
        public string name { get; set; }
        public string seasonID { get; set; }
        public List<Club> clubs { get; set; }

    }

    public class Club
    {
        public string id { get; set; }
        public string name { get; set; }
    }



}
