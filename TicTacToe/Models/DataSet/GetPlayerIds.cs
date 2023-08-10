﻿namespace TicTacToe.Models.DataSet
{
    public class GetPlayerIds
    {
        public List<object> errors { get; set; }
        public int results { get; set; }
        public List<Response> response { get; set; }
    }

    public class Response
    {
        public ApiPlayer player { get; set; }
    }
}