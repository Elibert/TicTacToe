namespace DataSetApi.Models
{
    public class Teams
    {
        public Api api { get; set; }
    }
    public class Api
    {
        public int results { get; set; }
        public List<Team> teams { get; set; }
    }

    public class Team
    {
        public string team_id { get; set; }
        public string name { get; set; }
        public string logo { get; set; }
        public string code { get; set; }
    }
}
