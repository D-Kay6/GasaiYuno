using System.Threading.Tasks;

namespace GasaiYuno.Discord.Music.Interfaces.Lyrics
{
    public interface ILyricsService
    {
        Task<ILyricsOption[]> Search(string input, int maxOptions = 10);
        Task<ILyrics> Get(ILyricsOption selection);
    }
}