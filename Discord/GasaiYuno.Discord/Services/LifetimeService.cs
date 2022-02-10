using GasaiYuno.Discord.Listeners;
using GasaiYuno.Discord.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Services
{
    public class LifetimeService
    {
        public bool KeepAlive { get; private set; }

        private readonly DiscordConnectionClient _connection;
        private readonly ConnectionListener _connectionListener;
        private readonly ILogger<LifetimeService> _logger;
        
        private CancellationTokenSource _stopTokenSource;

        /// <summary>
        /// Creates a new <see cref="LifetimeService"/>.
        /// </summary>
        /// <param name="connection">The <see cref="DiscordConnectionClient"/> used during this lifetime.</param>
        /// <param name="connectionListener">The <see cref="ConnectionListener"/> to start.</param>
        /// <param name="logger">The <see cref="ILogger{T}"/> that will log all the log messages.</param>
        public LifetimeService(DiscordConnectionClient connection, ConnectionListener connectionListener, ILogger<LifetimeService> logger)
        {
            _connection = connection;
            _connectionListener = connectionListener;
            _logger = logger;

            KeepAlive = true;
        }
        
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _stopTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            
            if (_stopTokenSource.IsCancellationRequested)
            {
                KeepAlive = false;
                return;
            }

            _connectionListener.Start();
            var result = await _connection.StartAsync().ConfigureAwait(false);
            if (!result)
            {
                KeepAlive = false;
                return;
            }

            try
            {
                await Task.Delay(Timeout.Infinite, _stopTokenSource.Token).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.LogInformation(e, "Cancellation of the lifetime was requested. Restarting: {KeepAlive}", KeepAlive);
            }
        }
        
        public async Task RestartAsync()
        {
            await _connection.StopAsync().ConfigureAwait(false);
            KeepAlive = true;
            _stopTokenSource.Cancel();
        }
        
        public async Task StopAsync()
        {
            await _connection.StopAsync().ConfigureAwait(false);
            KeepAlive = false;
            _stopTokenSource.Cancel();
        }
    }
}