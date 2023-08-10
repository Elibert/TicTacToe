using Microsoft.AspNetCore.Mvc;
using System.Web.Http;
using System.Net.Http.Headers;
using DataSetApi.Models;
using Newtonsoft.Json;

namespace DataSetApi.Controllers
{
    public class GetDataController : IGetData
    {
        private IConfiguration _config;
        public GetDataController(IConfiguration config)
        {
            _config = config;
        }
        public  async Task<Teams> GetTeamsByLeague(int leagueId)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://"+ _config.GetSection("ApiData:baseUrl").Value+ "/teams/league/" + leagueId),
                Headers =
                {
                    { "X-RapidAPI-Key", _config.GetSection("ApiData:key").Value },
                    { "X-RapidAPI-Host", _config.GetSection("ApiData:baseUrl").Value },
                },
            };
            using (var response = await client.SendAsync(request))
            {
                var body = await response.Content.ReadAsStringAsync();
                Teams result = JsonConvert.DeserializeObject<Teams>(body);
                return result;
            }
        }
    }
}
