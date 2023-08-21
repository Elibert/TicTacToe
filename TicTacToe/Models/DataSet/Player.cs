namespace TicTacToe.Models.DataSet
{
    public class ApiPlayer
    {
        public string url { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string fullname { get; set; }

        public string nameInHomeCountry { get; set; }
        public string imageURL { get; set; }
        public string dateOfBirth { get; set; }
        public string age { get; set; }
        public string height { get; set; }
        public List<string> citizenship { get; set; }
        public bool isRetired { get; set; }
        public string retiredSince { get; set; }
        public string foot { get; set; }
        public Club club { get; set; }
        public DateTime lastUpdate { get; set; }
    }
    public class Birth
    {
        public string date { get; set; }
        public string place { get; set; }
        public string country { get; set; }
    }
}
