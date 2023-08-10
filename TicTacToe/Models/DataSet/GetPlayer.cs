namespace TicTacToe.Models.DataSet
{
    public class GetPlayer
    {
        public List<object> errors { get; set; }
        public int results { get; set; }
        public List<GetPlayerResponse> response { get; set; }
    }

    public class GetPlayerResponse
    {
        public Team team { get; set; }
        public ApiPlayer player { get; set; }
    }
}
