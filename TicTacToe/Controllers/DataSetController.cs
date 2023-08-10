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
        public void CreateClubDataSet()
        {
            var allLeagueClubs = _getData.GetTeamsByLeague(135);
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
            foreach(var club in _context.Clubs.ToList())
            {
                var allteamPlayers = _getData.GetPlayerIdsByTeam(Convert.ToInt32(club.ApiTeamId));
                foreach (var player in allteamPlayers.Result.response)
                {
                    Player playerModel = new Player();
                    playerModel.ApiPlayerId = player.player.id.ToString();
                    playerModel.PlayerName = player.player.firstname.Split(" ")[0] + " " + player.player.lastname;
                    _context.Players.Add(playerModel);
                }
            }
            _context.SaveChanges();
        }

        public void CreatePlayerClubHistoryDataSet()
        {
            foreach (var player in _context.Players.ToList())
            {
                var allhistoryPlayer = _getData.GetPlayerClubHistory(Convert.ToInt32(player.ApiPlayerId));
                foreach (var history in allhistoryPlayer.Result.response.First().transfers)
                {
                    PlayerClubHistory playerModel = new PlayerClubHistory();
                    playerModel.PlayerId = _context.Players.Where(x => x.ApiPlayerId == allhistoryPlayer.Result.response.First().player.id.ToString()).First().PlayerId;
                    playerModel.ClubId = _context.Clubs.Where(x => x.ApiTeamId == history.teams.@out.id.ToString()).First().ClubId;
                    _context.PlayerClubHistories.Add(playerModel);

                    //get both the "in" and "out" club if it is the first transfer
                    if (allhistoryPlayer.Result.response.First().transfers.First() == history)
                    {
                        PlayerClubHistory playerH = new PlayerClubHistory();
                        playerH.PlayerId = _context.Players.Where(x => x.ApiPlayerId == allhistoryPlayer.Result.response.First().player.id.ToString()).First().PlayerId;
                        playerH.ClubId = _context.Clubs.Where(x => x.ApiTeamId == history.teams.@in.id.ToString()).First().ClubId;
                        _context.PlayerClubHistories.Add(playerH);
                    }
                }
            }
            _context.SaveChanges();
        }
    }
}
