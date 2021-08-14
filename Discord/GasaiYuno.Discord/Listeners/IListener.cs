using System.Threading.Tasks;

namespace GasaiYuno.Discord.Listeners
{
    public interface IListener
    {
        Task OnReady();
    }
}