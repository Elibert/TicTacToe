namespace TicTacToe.Models.DataSet
{
    public class GetPlayerIds
    {
        public string id { get; set; }
        public string clubName { get; set; }
        public string seasonYear { get; set; }
        public List<ApiPlayer> players { get; set; }
    }
}
