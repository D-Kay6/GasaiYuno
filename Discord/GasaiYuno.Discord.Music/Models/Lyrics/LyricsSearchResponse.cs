using GasaiYuno.Discord.Music.Interfaces.Lyrics;
using Newtonsoft.Json;

namespace GasaiYuno.Discord.Music.Models.Lyrics
{
    public class LyricsSearchResponse
    {
        [JsonProperty("hits")]
        public LyricsHit[] Hits { get; init; }
    }

    public class LyricsHit
    {
        [JsonProperty("highlights")]
        public string[] Highlights { get; init; }

        [JsonProperty("index")]
        public string Index { get; init; }

        [JsonProperty("type")]
        public string Type { get; init; }

        [JsonProperty("result")]
        public LyricsHitResult Result { get; init; }
    }

    public class LyricsHitResult : ILyricsOption
    {
        [JsonIgnore]
        public string Id => OriginalId.ToString();

        [JsonProperty("id")]
        internal int OriginalId { get; init; }

        [JsonProperty("title")]
        public string Title { get; init; }

        [JsonProperty("full_title")]
        public string FullTitle { get; init; }

        public string Artist => PrimaryArtist?.Name;

        [JsonProperty("url")]
        public string Url { get; init; }

        [JsonProperty("path")]
        public string Path { get; init; }

        [JsonProperty("api_path")]
        public string ApiPath { get; init; }

        [JsonProperty("primary_artist")]
        public LyricsHitArtist PrimaryArtist { get; init; }
    }

    public class LyricsHitArtist
    {
        [JsonIgnore]
        public string Id => OriginalId.ToString();

        [JsonProperty("id")]
        internal int OriginalId { get; init; }

        [JsonProperty("name")]
        public string Name { get; init; }

        [JsonProperty("url")]
        public string Url { get; init; }

        [JsonProperty("api_path")]
        public string ApiPath { get; init; }
    }
}