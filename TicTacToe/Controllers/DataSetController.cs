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
    }
}
