using Microsoft.AspNetCore.Mvc;
using TicTacToe.Data;
using TicTacToe.DataSet;
using TicTacToe.Models;

namespace TicTacToe.Controllers
{
    public class DataSetController : Controller
    {
        private readonly TictactoeContext _context;
        private readonly IGetData _getData;

        public DataSetController(TictactoeContext context, IGetData getData)
        {
            _context = context;
            _getData = getData;
        }
        public void CreateClubDataSet(string id)
        {
            var allLeagueClubs = _getData.GetTeamsByLeague(id);
            foreach(var club in allLeagueClubs.Result.clubs)
            {
                Club clubModel = new Club();
                clubModel.ClubName = club.name;
                clubModel.ApiTeamId = club.id;
                _context.Clubs.Add(clubModel);
            }
            _context.SaveChanges();
        }

        public void ClubProfileDataSet()
        {
            foreach (var club in _context.Clubs.ToList())
            {
                var clubProfile = _getData.GetTeamsProfile(club.ApiTeamId);
                club.ClubLogo = clubProfile.Result.image;
            }
            _context.SaveChanges();
        }

        public void CreatePlayerIdDataSet()
        {
            foreach(var club in _context.Clubs.ToList())
            {
                var allteamPlayers = _getData.GetPlayerIdsByTeam(Convert.ToInt32(club.ApiTeamId));
                foreach (var player in allteamPlayers.Result.players)
                {
                    Player playerModel = new Player();
                    playerModel.ApiPlayerId = player.id.ToString();
                    playerModel.Birthdate = Convert.ToDateTime(String.Format("{0:MM/dd/yyyy}", player.dateOfBirth));
                    playerModel.PlayerName = player.name;
                    _context.Players.Add(playerModel);
                }
            }
            _context.SaveChanges();
        }

        //public void FillPlayerDetailsDataSet()
        //{
        //    foreach (var player in _context.Players.ToList())
        //    {
        //        var playerDetails = _getData.GetPlayerDetails(Convert.ToInt32(player.ApiPlayerId));
        //        player.PlayerName = playerDetails.Result.response.First().player.firstname.Split(' ')[0] + " " + playerDetails.Result.response.First().player.lastname;
        //    }
        //    _context.SaveChanges();
        //}

        public void CreatePlayerClubHistoryDataSet()
        {
            try
            {
                foreach (var player in _context.Players.ToList())
                {
                    var allhistoryPlayer = _getData.GetPlayerClubHistory(Convert.ToInt32(player.ApiPlayerId));
                    if (allhistoryPlayer.Result.history !=null && allhistoryPlayer.Result.history.Count > 0)
                    {
                        bool firstTransfer = true;
                        foreach (var history in allhistoryPlayer.Result.history)
                        {
                            if (_context.Clubs.Where(x => x.ApiTeamId == history.oldClubID).Count() > 0)
                            {
                                PlayerClubHistory playerModel = new PlayerClubHistory();
                                playerModel.PlayerId = player.PlayerId;
                                playerModel.ClubId = _context.Clubs.Where(x => x.ApiTeamId == history.oldClubID).First().ClubId;
                                if (_context.PlayerClubHistories.Where(x => x.ClubId == playerModel.ClubId && x.PlayerId == playerModel.PlayerId).Count() == 0)
                                    _context.PlayerClubHistories.Add(playerModel);
                            }

                            //get both the "in" and "out" club if it is the first transfer
                            if (firstTransfer && _context.Clubs.Where(x => x.ApiTeamId == history.newClubID).Count() > 0)
                            {
                                PlayerClubHistory playerH = new PlayerClubHistory();
                                playerH.PlayerId = player.PlayerId;
                                playerH.ClubId = _context.Clubs.Where(x => x.ApiTeamId == history.newClubID).First().ClubId;
                                _context.PlayerClubHistories.Add(playerH);
                                firstTransfer = false;
                            }
                        }
                    }
                }
                _context.SaveChanges();

            }
            catch (Exception ex)
            {
                _context.SaveChanges();
                        

        public void CreatePlayerId()
        {
            List<int> retiredPlayers = new() { 65230,25557,26399,112465,5819,7912,30638,7717,5354,5962,7840,5759,6031,7600,58205,45403,43907,203412,5817,76061,50219,2540,5841,5922,5866,
                                               3524,131789,7518,3185,34870,112302,75,15185,29835,266302,7589,28021,5299,7427,92141,33873,45146,74418,2904,8024,5794,6081,318077,77,77100,
                                               26721,41414,5588,179184,1397,88994,2963,12142};
            foreach (int id in retiredPlayers)
            {
                var allteamPlayers = _getData.GetPlayersById(id);

                Player playerModel = new Player();
                playerModel.ApiPlayerId = id.ToString();
                playerModel.Birthdate = Convert.ToDateTime(allteamPlayers.Result.dateOfBirth);
                playerModel.PlayerName = !String.IsNullOrWhiteSpace(allteamPlayers.Result.nameInHomeCountry) ? allteamPlayers.Result.nameInHomeCountry : !String.IsNullOrWhiteSpace(allteamPlayers.Result.fullname) ? allteamPlayers.Result.fullname : allteamPlayers.Result.name;
                _context.Players.Add(playerModel);
            }
            _context.SaveChanges();
        }
    }
}
