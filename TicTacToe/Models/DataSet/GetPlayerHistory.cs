using System.Net.NetworkInformation;
using System.Security.Cryptography.Xml;

namespace TicTacToe.Models.DataSet
{
    public class GetPlayerHistory
    {
        public int results { get; set; }
        public List<PlayerHistory> response { get; set; }
    }
    public class PlayerHistory
    {
        public ApiPlayer player { get; set; }
        public DateTime update { get; set; }
        public List<Transfer> transfers { get; set; }
    }
    public class Transfer
    {
        public string date { get; set; }
        public string type { get; set; }
        public Teams teams { get; set; }
    }

    public class Teams
    {
        public History @in { get; set; }
        public History @out { get; set; }
    }

    public class History
    {
        public int? id { get; set; }
        public string name { get; set; }
        public string logo { get; set; }
    }

}
