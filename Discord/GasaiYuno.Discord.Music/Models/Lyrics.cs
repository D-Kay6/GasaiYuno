using GasaiYuno.Interface.Music;

namespace GasaiYuno.Discord.Music.Models
{
    public class Lyrics : ILyrics
    {
        public string Content { get; internal set; }
        public ILyricsPart[] Parts { get; internal set; }
    }

    public class LyricsPart : ILyricsPart
    {
        public string Title { get; internal set; }
        public string Content { get; internal set; }
    }
}