using TicTacToe.Models.DataSet;

namespace TicTacToe.DataSet
{
    public interface IGetData
    {
        public Task<GetTeams> GetTeamsByLeague(int leagueId);
    }
}
