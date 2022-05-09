using GasaiYuno.Discord.Music.Interfaces.Lyrics;

namespace GasaiYuno.Discord.Music.Models.Lyrics;

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