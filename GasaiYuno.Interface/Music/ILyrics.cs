namespace GasaiYuno.Interface.Music
{
    public interface ILyrics
    {
        string Content { get; }
        ILyricsPart[] Parts { get; }
    }

    public interface ILyricsPart
    {
        string Title { get; }
        string Content { get; }
    }
}