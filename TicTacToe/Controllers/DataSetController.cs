using Microsoft.AspNetCore.Mvc;
using TicTacToe.Data;
using DataSetApi.Controllers;
using DataSetApi.Models;
using TicTacToe.Models;

namespace TicTacToe.Controllers
{
    public class DataSetController : Controller
    {
        private readonly TictactoeContext _context;
        public DataSetController(TictactoeContext context)
        {
            _context = context;
        }
        public void CreateClubDataSet()
        {
            var allLeagueClubs = GetDataController.GetTeamsByLeague(135);
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
