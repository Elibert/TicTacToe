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
        public void CreateClubDataSet(int leagueId)
        {
            var allLeagueClubs = _getData.GetTeamsByLeague(leagueId);
            foreach(var club in allLeagueClubs.Result.api.teams)
            {
                Club clubModel = new Club();
                clubModel.ClubLogo = club.logo;
                clubModel.ClubName = club.name;
                clubModel.ApiTeamId = club.team_id;
                _context.Clubs.Add(clubModel);
            }
            _context.SaveChanges();
        }

        public void CreatePlayerIdDataSet()
        {
            int i=1;
            foreach(var club in _context.Clubs.ToList())
            {
                if (i <= 30)
                {
                    var allteamPlayers = _getData.GetPlayerIdsByTeam(Convert.ToInt32(club.ApiTeamId));
                    i++;
                    foreach (var player in allteamPlayers.Result.response.First().players)
                    {
                        Player playerModel = new Player();
                        playerModel.ApiPlayerId = player.id.ToString();
                        playerModel.PlayerName = player.name;
                        _context.Players.Add(playerModel);
                    }
                }
                else
                {
                    break;
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
        //        player.Birthdate = DateTime.ParseExact(playerDetails.Result.response.First().player.birth.date, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
        //    }
        //    _context.SaveChanges();
        //}

        public void CreatePlayerClubHistoryDataSet()
        {
            int i = 1;
            foreach (var player in _context.Players.ToList().Where(p=>p.PlayerId> 596))
            {
                if (i <= 13)
                {
                    var allhistoryPlayer = _getData.GetPlayerClubHistory(Convert.ToInt32(player.ApiPlayerId));
                    i++;
                    if (allhistoryPlayer.Result.response.Count > 0 && allhistoryPlayer.Result != null && allhistoryPlayer.Result.response!=null)
                    {
                        bool firstTransfer = true;
                        foreach (var history in allhistoryPlayer.Result.response.First().transfers)
                        {
                            if (_context.Clubs.Where(x => x.ApiTeamId == history.teams.@out.id.ToString()).Count() > 0)
                            {
                                PlayerClubHistory playerModel = new PlayerClubHistory();
                                playerModel.PlayerId = player.PlayerId;
                                playerModel.ClubId = _context.Clubs.Where(x => x.ApiTeamId == history.teams.@out.id.ToString()).First().ClubId;
                                if(!_context.PlayerClubHistories.Any(p=>p.PlayerId==playerModel.PlayerId && p.ClubId==playerModel.ClubId))
                                    _context.PlayerClubHistories.Add(playerModel);
                            }

                            //get both the "in" and "out" club if it is the first transfer
                            if (firstTransfer && _context.Clubs.Where(x => x.ApiTeamId == history.teams.@in.id.ToString()).Count() > 0)
                            {
                                PlayerClubHistory playerH = new PlayerClubHistory();
                                playerH.PlayerId = player.PlayerId;
                                playerH.ClubId = _context.Clubs.Where(x => x.ApiTeamId == history.teams.@in.id.ToString()).First().ClubId;
                                if (!_context.PlayerClubHistories.Any(p => p.PlayerId == playerH.PlayerId && p.ClubId == playerH.ClubId))
                                    _context.PlayerClubHistories.Add(playerH);
                                firstTransfer = false;
                            }
                        }
                        _context.SaveChanges();
                    }
                }
                else
                {
                    break;
                }
            }
        }

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
