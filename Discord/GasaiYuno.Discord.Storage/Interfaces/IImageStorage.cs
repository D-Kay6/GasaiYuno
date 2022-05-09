using System.Threading.Tasks;

namespace GasaiYuno.Discord.Storage.Interfaces;

public interface IImageStorage
{
    Task<string> GetImageAsync(string name, string directory = null);
    Task<string> SaveImageAsync(string url, string directory);
    Task DeleteImageAsync(string path);
}