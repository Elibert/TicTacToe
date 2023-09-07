using System.Security.Cryptography;
using System.Text;
using TicTacToe.Data;
using TicTacToe.Models;

namespace TicTacToe.Helpers
{
    public class FunctionHelper
    {
        public static string GenerateCode(int length, TictactoeContext _context)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            using (var rng = RandomNumberGenerator.Create())
            {
                StringBuilder result = new StringBuilder(length);
                do
                {
                    byte[] data = new byte[length];
                    rng.GetBytes(data);
                    foreach (byte b in data)
                    {
                        result.Append(chars[b % chars.Length]);
                    }
                }while (_context.Games.Any(g=>g.GameCode==result.ToString()));
                return result.ToString();
            }
        }
        public static List<int> CheckIfThereIsAnyWinner(List<RoundMove> gameMoves, TicTacToeTypes player)
        {
            string[] boardArray = new string[9];
            int counterIndex = 0;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    List<RoundMove> moves = gameMoves.Where(gm => gm.RowNo == i && gm.ColNo == j).ToList();
                    boardArray[counterIndex] = moves.Count() > 0 ? moves.First().CellValue : "";
                    counterIndex++;
                }
            }

            int[,] winCombinations = new int[8, 3] { { 0, 1, 2 }, { 3, 4, 5 }, { 6, 7, 8 }, { 0, 3, 6 }, { 1, 4, 7 }, { 2, 5, 8 }, { 0, 4, 8 }, { 2, 4, 6 } };
            List<int> possibleWinnigCombination = new List<int>();
            for (int i = 0; i < 8; i++)
            {
                possibleWinnigCombination.Clear();
                int counter = 0;
                for (int j = 0; j < 3; j++)
                {
                    if (boardArray[winCombinations[i, j]] == player.ToString())
                    {
                        possibleWinnigCombination.Add(winCombinations[i, j]);
                        counter++;
                        if (counter == 3)
                        {
                            return possibleWinnigCombination;
                        }
                    }
                }
            }
            return possibleWinnigCombination;
        }

        public static void ChangeUserConnectionId(string user,string newConnId, TictactoeContext _context)
        {
            string[] userInfo = user.Split("_");
            User userToChange = _context.Users.Where(u => u.UserName == userInfo[0] && u.UserId == Convert.ToInt32(userInfo[1])).First();

            if(userToChange is object)
            {
                if(_context.UserConnections.Where(uc=>uc.UserId==userToChange.UserId).Count()>0)
                {
                    UserConnection userConnection = _context.UserConnections.Where(uc => uc.UserId == userToChange.UserId).First();
                    userConnection.ConnectionId = newConnId;
                    _context.SaveChanges();
                }
            }
        }
    }
}
