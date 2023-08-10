namespace DataSetApi.Models
{
    public interface IGetData
    {
        public Task<Teams> GetTeamsByLeague(int leagueId);
    }
}
