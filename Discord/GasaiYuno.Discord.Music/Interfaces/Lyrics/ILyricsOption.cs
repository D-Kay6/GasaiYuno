namespace GasaiYuno.Discord.Music.Interfaces.Lyrics;

public interface ILyricsOption
{
    public string Id { get; }
    public string Title { get; }
    public string FullTitle { get; }
    public string Artist { get; }
    public string Url { get; }
}