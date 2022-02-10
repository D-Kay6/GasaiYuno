using GasaiYuno.Discord.Music.Interfaces.Lyrics;
using GasaiYuno.Discord.Music.Models.Lyrics;
using HtmlAgilityPack;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators.OAuth2;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Music.Services
{
    public class LyricsService : ILyricsService
    {
        private readonly RestClient _restClient;
        private const string LyricsIndexerStart = "window.__PRELOADED_STATE__ = JSON.parse('";
        private const string LyricsIndexerEnd = "');";

        public LyricsService(string token)
        {
            _restClient = new RestClient("https://api.genius.com/")
            {
                Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(token, "Bearer")
            };
        }

        public async Task<ILyricsOption[]> Search(string song, int maxOptions = 10)
        {
            if (maxOptions > 50) maxOptions = 50;
            var response = await _restClient.ExecuteGetAsync(new RestRequest($"search?q={song}&per_page={maxOptions}"));
            var lyricsResult = JsonConvert.DeserializeObject<LyricsResult<LyricsSearchResponse>>(response.Content);
            return lyricsResult?.Response.Hits.Select(x => x.Result as ILyricsOption).ToArray();
        }

        public async Task<ILyrics> Get(ILyricsOption selection)
        {
            string[] lyricsSections;
            var htmlDocument = new HtmlDocument();
            var response = await _restClient.ExecuteGetAsync(new RestRequest(new Uri(selection.Url)));
            var startIndex = response.Content.IndexOf(LyricsIndexerStart, StringComparison.InvariantCultureIgnoreCase);
            if (startIndex < 0)
            {
                htmlDocument.LoadHtml(response.Content);
                var htmlNode = htmlDocument.DocumentNode.SelectSingleNode("//div[@class='lyrics']");
                lyricsSections = htmlNode?.InnerText?.Split("\n\n", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            }
            else
            {
                startIndex += LyricsIndexerStart.Length;
                var stopIndex = response.Content.IndexOf(LyricsIndexerEnd, startIndex, StringComparison.InvariantCulture);
                var line = response.Content.Substring(startIndex, stopIndex - startIndex);
                dynamic data = JsonConvert.DeserializeObject(Regex.Unescape(line));
                string lyricsHtml = data?.songPage?.lyricsData?.body?.html;
                
                htmlDocument.LoadHtml(lyricsHtml);
                lyricsSections = htmlDocument.DocumentNode.InnerText.Split("\n\n", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            }
            if (lyricsSections == null || lyricsSections.Length == 0) return null;

            Lyrics result = new Lyrics
            {
                Parts = new ILyricsPart[lyricsSections.Length]
            };
            for (var i = 0; i < lyricsSections.Length; i++)
            {
                var lines = lyricsSections[i].Split("\n", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                var skipAmount = 0;
                var lyricsPart = new LyricsPart();
                if (Regex.IsMatch(lines[0], @"\[.*\]"))
                {
                    lyricsPart.Title = lines[0];
                    skipAmount = 1;
                }

                lyricsPart.Content = string.Join(Environment.NewLine, lines.Skip(skipAmount));
                result.Parts[i] = lyricsPart;
            }
            if (result.Parts.All(x => string.IsNullOrWhiteSpace(x.Title)) || result.Parts.Any(x => x.Content.Length > 1024))
            {
                for (var i = 0; i < lyricsSections.Length; i++)
                {
                    var lines = lyricsSections[i].Split("\n", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    var skipAmount = Regex.IsMatch(lines[0], @"\[.*\]") ? 1 : 0;
                    lyricsSections[i] = string.Join(Environment.NewLine, lines.Skip(skipAmount));
                }

                result.Content = string.Join(Environment.NewLine + Environment.NewLine, result.Parts.Select(x => x.Content));
                result.Parts = null;
            }

            return result;
        }
    }
}