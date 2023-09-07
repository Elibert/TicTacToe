using System.Security.Cryptography;
using System.Text;
using TicTacToe.Data;
using TicTacToe.Models;

namespace TicTacToe.Helpers
{
    public class FunctionHelper
    {
        private static TictactoeContext context;
        private static RSACryptoServiceProvider csp = new RSACryptoServiceProvider(2048);
        private static RSAParameters privKey = csp.ExportParameters(true);
        private static RSAParameters pubKey = csp.ExportParameters(false);

        public FunctionHelper(TictactoeContext tictactoeContext)
        {
            context = tictactoeContext;
        }

        public string GenerateCode(int length)
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
                }while (context.Games.Any(g=>g.GameCode==result.ToString()));
                return result.ToString();
            }
        }
        public List<int> CheckIfThereIsAnyWinner(List<RoundMove> gameMoves, TicTacToeTypes player)
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

        public static void ChangeUserConnectionId(string cookieValue,string newConnId)
        {
            string user = EncryptDecryptValue(false, cookieValue);
            string[] userInfo = user.Split("_");
            if (userInfo.Length == 2)
            {
                User userToChange = context.Users.Where(u => u.UserName == userInfo[0] && u.UserId == Convert.ToInt32(userInfo[1])).First();

                if(userToChange is object)
                {
                    if (context.UserConnections.Where(uc => uc.UserId == userToChange.UserId).Count() > 0)
                    {
                        UserConnection userConnection = context.UserConnections.Where(uc => uc.UserId == userToChange.UserId).First();
                        userConnection.ConnectionId = newConnId;
                        context.SaveChanges();
                    }
                }
            }
        }

        public static string EncryptDecryptValue(bool isEncrypt,string cookieValue)
        {
            if (isEncrypt)
            {
                string pubKeyString;
                {
                    var sw = new System.IO.StringWriter();
                    var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
                    xs.Serialize(sw, pubKey);
                    pubKeyString = sw.ToString();
                }
                {
                    var sr = new System.IO.StringReader(pubKeyString);
                    var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
                    pubKey = (RSAParameters)xs.Deserialize(sr);
                }
                RSACryptoServiceProvider csp = new RSACryptoServiceProvider();
                csp.ImportParameters(pubKey);
                byte[] bytesPlainTextData = Encoding.Unicode.GetBytes(cookieValue);
                byte[] bytesCypherText = csp.Encrypt(bytesPlainTextData, false);
                return Convert.ToBase64String(bytesCypherText);
            }
            else
            {
                byte[] bytesCypherText = Convert.FromBase64String(cookieValue);
                RSACryptoServiceProvider csp = new RSACryptoServiceProvider();
                csp.ImportParameters(privKey);
                byte[] bytesPlainTextData = csp.Decrypt(bytesCypherText, false);
                return Encoding.Unicode.GetString(bytesPlainTextData);
            }
        }
    }
}
