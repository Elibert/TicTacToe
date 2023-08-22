using Newtonsoft.Json;
using TicTacToe.Models.DataSet;

namespace TicTacToe.DataSet
{
    public class GetDataController : IGetData
    {
        private IConfiguration _config;
        public GetDataController(IConfiguration config)
        {
            _config = config;
        }
        public async Task<GetTeams> GetTeamsByLeague(string leagueId)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(_config.GetSection("ApiData:baseUrl").Value + "/competitions/"+ leagueId+ "/clubs"),
            };
            using (var response = await client.SendAsync(request))
            {
                var body = await response.Content.ReadAsStringAsync();
                GetTeams result = JsonConvert.DeserializeObject<GetTeams>(body);
                return result;
            }
        }

        public async Task<ClubProfile> GetTeamsProfile(string clubId)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(_config.GetSection("ApiData:baseUrl").Value + "/clubs/" + clubId + "/profile"),
            };
            using (var response = await client.SendAsync(request))
            {
                var body = await response.Content.ReadAsStringAsync();
                ClubProfile result = JsonConvert.DeserializeObject<ClubProfile>(body);
                return result;
            }
        }

        public async Task<GetPlayerIds> GetPlayerIdsByTeam(int teamId)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(_config.GetSection("ApiData:baseUrl").Value + "/clubs/" + teamId+"/players"),
            };
            using (var response = await client.SendAsync(request))
            {
                var body = await response.Content.ReadAsStringAsync();
                GetPlayerIds result = JsonConvert.DeserializeObject<GetPlayerIds>(body);
                return result;
            }
        }

        public async Task<GetPlayer> GetPlayerDetails(int playerId)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://" + _config.GetSection("ApiData:baseUrl").Value + "/v3/players=" + playerId + "&season=" + DateTime.Today.Year),
                Headers =
            {
                { "X-RapidAPI-Key", _config.GetSection("ApiData:key").Value },
                { "X-RapidAPI-Host", _config.GetSection("ApiData:baseUrl").Value },
            },
            };
            using (var response = await client.SendAsync(request))
            {
                var body = await response.Content.ReadAsStringAsync();
                GetPlayer result = JsonConvert.DeserializeObject<GetPlayer>(body);
                return result;
            }
        }

        public async Task<GetPlayerHistory> GetPlayerClubHistory(int playerId)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(_config.GetSection("ApiData:baseUrl").Value + "/players/" + playerId+ "/transfers"),

            };
            using (var response = await client.SendAsync(request))
            {
                var body = await response.Content.ReadAsStringAsync();
                GetPlayerHistory result = JsonConvert.DeserializeObject<GetPlayerHistory>(body);
                return result;
            }
        }

        public async Task<ApiPlayer> GetPlayersById(int playerId)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(_config.GetSection("ApiData:baseUrl").Value + "/players/" + playerId + "/profile"),
            };
            using (var response = await client.SendAsync(request))
            {
                var body = await response.Content.ReadAsStringAsync();
                ApiPlayer result = JsonConvert.DeserializeObject<ApiPlayer>(body);
                return result;
            }
        }
    }
}
