using TicTacToe.Models.DataSet;

namespace TicTacToe.DataSet
{
    public interface IGetData
    {
        public Task<GetTeams> GetTeamsByLeague(int leagueId);

        public Task<GetPlayerIds> GetPlayerIdsByTeam(int leagueId);

        public Task<GetPlayerHistory> GetPlayerClubHistory(int playerId);

        public Task<GetPlayer> GetPlayerDetails(int playerId);

        public Task<ApiPlayer> GetPlayersById(int playerId);
    }
}
