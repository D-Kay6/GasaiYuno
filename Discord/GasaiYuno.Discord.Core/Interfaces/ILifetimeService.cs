using System.Threading;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Core.Interfaces;

public interface ILifetimeService
{
    bool KeepAlive { get; }
    Task StartAsync(CancellationToken cancellationToken);

    Task RestartAsync();

    Task StopAsync();
}