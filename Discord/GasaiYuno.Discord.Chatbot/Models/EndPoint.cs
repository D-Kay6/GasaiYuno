using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace GasaiYuno.Discord.Chatbot.Models
{
    internal class EndPoint
    {
        private const string Url = "https://www.cleverbot.com/getreply?key={0}&input={1}";

        private readonly string _apiKey;

        public EndPoint(string apiKey)
        {
            _apiKey = apiKey;
        }

        public async Task<Reply> GetReplyAsync(string message, string state)
        {
            var url = string.Format(Url, _apiKey, HttpUtility.UrlEncode(message));
            if (!string.IsNullOrWhiteSpace(state))
                url += $"&cs={state}";

            using var httpClient = new HttpClient();
            var jsonString = await httpClient.GetStringAsync(url).ConfigureAwait(false);
            return JsonConvert.DeserializeObject<Reply>(jsonString);
        }
    }
}