namespace TicTacToe.Models.DataSet
{
    public class ClubProfile
    {
        public string id { get; set; }
        public string url { get; set; }
        public string name { get; set; }
        public string officialName { get; set; }
        public string image { get; set; }
        public string legalForm { get; set; }
        public string addressLine1 { get; set; }
        public string addressLine2 { get; set; }
        public string addressLine3 { get; set; }
        public string tel { get; set; }
        public string fax { get; set; }
        public string website { get; set; }
        public string foundedOn { get; set; }
        public string members { get; set; }
        public string membersDate { get; set; }
        public List<string> colors { get; set; }
        public string stadiumName { get; set; }
        public string stadiumSeats { get; set; }
        public string currentTransferRecord { get; set; }
        public string currentMarketValue { get; set; }
        public Squad squad { get; set; }
        public League league { get; set; }
        public List<string> historicalCrests { get; set; }
    }

    public class League
    {
        public string id { get; set; }
        public string name { get; set; }
        public string countryID { get; set; }
        public string countryName { get; set; }
        public string tier { get; set; }
    }
    public class Squad
    {
        public string size { get; set; }
        public string averageAge { get; set; }
        public string foreigners { get; set; }
        public string nationalTeamPlayers { get; set; }
    }


}
