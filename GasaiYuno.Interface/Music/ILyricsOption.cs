namespace GasaiYuno.Interface.Music
{
    public interface ILyricsOption
    {
        public string Id { get; }
        public string Title { get; }
        public string FullTitle { get; }
        public string Artist { get; }
        public string Url { get; }
    }
}