using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Services
{
    public class RestartService
    {
        private readonly ILogger<RestartService> _logger;
        private CancellationTokenSource _restartToken;

        public bool KeepAlive { get; private set; }

        public RestartService(ILogger<RestartService> logger)
        {
            _logger = logger;
            KeepAlive = true;
        }

        public void Restart()
        {
            _restartToken?.Cancel();
        }

        public void Shutdown()
        {
            KeepAlive = false;
            _restartToken?.Cancel();
        }

        public async Task AwaitRestart()
        {
            _restartToken?.Dispose();
            _restartToken = new CancellationTokenSource();
            try
            {
                await Task.Delay(-1, _restartToken.Token).ConfigureAwait(false);
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation("Stopped keeping the current client alive. Restarting: {KeepAlive}", KeepAlive);
            }
        }
    }
}