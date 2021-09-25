using Autofac;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Services
{
    public class DiscordService : BackgroundService
    {
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly ILifetimeScope _lifetimeScope;
        private readonly ILogger<DiscordService> _logger;

        /// <summary>
        /// Creates a new <see cref="DiscordService"/>.
        /// </summary>
        /// <param name="hostApplicationLifetime">The <see cref="IHostApplicationLifetime"/> that is used to shut the application down.</param>
        /// <param name="lifetimeScope">The <see cref="ILifetimeScope"/> that will create child scopes.</param>
        /// <param name="logger">The <see cref="ILogger{T}"/> that will log all the log messages.</param>
        public DiscordService(IHostApplicationLifetime hostApplicationLifetime, ILifetimeScope lifetimeScope, ILogger<DiscordService> logger)
        {
            _hostApplicationLifetime = hostApplicationLifetime;
            _lifetimeScope = lifetimeScope;
            _logger = logger;
        }
        
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var keepAlive = false;
            do
            {
                await using var scope = _lifetimeScope.BeginLifetimeScope("DiscordLifetime");
                var lifetimeService = scope.Resolve<LifetimeService>();
                await lifetimeService.StartAsync(cancellationToken);
                keepAlive = lifetimeService.KeepAlive;
            } while (keepAlive);
            //_hostApplicationLifetime.StopApplication();
        }
    }
}