using System.Threading.Tasks;

namespace GasaiYuno.Discord.Handlers
{
    public interface IHandler
    {
        Task Ready();
    }
}