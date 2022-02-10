using Newtonsoft.Json;

namespace GasaiYuno.Discord.Music.Models.Lyrics
{
    public class LyricsResult<T>
    {
        [JsonProperty("meta")]
        public Meta Meta { get; init; }

        [JsonProperty("response")]
        public T Response { get; init; }
    }
    public class Meta
    {
        [JsonProperty("status")]
        public int Status { get; init; }
    }
}