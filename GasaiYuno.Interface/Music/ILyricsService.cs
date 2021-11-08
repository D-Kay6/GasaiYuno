using System.Threading.Tasks;

namespace GasaiYuno.Interface.Music
{
    public interface ILyricsService
    {
        Task<ILyricsOption[]> Search(string input, int maxOptions = 10);
        Task<ILyrics> Get(ILyricsOption selection);
    }
}