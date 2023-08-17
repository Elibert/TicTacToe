using System.Net.NetworkInformation;
using System.Security.Cryptography.Xml;

namespace TicTacToe.Models.DataSet
{
    public class GetPlayerHistory
    {
        public string id { get; set; }
        public string name { get; set; }
        public List<History> history { get; set; }
        public List<string> youthClubs { get; set; }
        public DateTime lastUpdate { get; set; }
    }
    public class History
    {
        public string transferID { get; set; }
        public string transferSeason { get; set; }
        public string transferDate { get; set; }
        public string oldClubID { get; set; }
        public string oldClubName { get; set; }
        public string newClubID { get; set; }
        public string newClubName { get; set; }
        public string marketValue { get; set; }
        public string fee { get; set; }
    }

}
