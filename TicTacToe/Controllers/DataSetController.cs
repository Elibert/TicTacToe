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
            int i=1;
            foreach(var club in _context.Clubs.ToList())
            {
                var allteamPlayers = _getData.GetPlayerIdsByTeam(Convert.ToInt32(club.ApiTeamId));
                foreach (var player in allteamPlayers.Result.players)
                {
                    Player playerModel = new Player();
                    playerModel.ApiPlayerId = player.id.ToString();
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
                    if (allhistoryPlayer.Result.history.Count > 0)
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
            }
        }
    }
}
