using Autofac;
using GasaiYuno.Discord.Core.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GasaiYuno.Discord.Services;

public class DiscordService : BackgroundService
{
    private readonly ILifetimeScope _lifetimeScope;
    private readonly ILogger<DiscordService> _logger;

    /// <summary>
    /// Creates a new <see cref="DiscordService"/>.
    /// </summary>
    /// <param name="lifetimeScope">The <see cref="ILifetimeScope"/> that will create child scopes.</param>
    /// <param name="logger">The <see cref="ILogger{T}"/> that will log all the log messages.</param>
    public DiscordService(ILifetimeScope lifetimeScope, ILogger<DiscordService> logger)
    {
        _lifetimeScope = lifetimeScope;
        _logger = logger;
    }
        
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var keepAlive = false;
        do
        {
            try
            {
                var scope = _lifetimeScope.BeginLifetimeScope("DiscordLifetime");
                await using (scope.ConfigureAwait(false))
                {
                    var lifetimeService = scope.Resolve<ILifetimeService>();
                    await lifetimeService.StartAsync(cancellationToken).ConfigureAwait(false);
                    keepAlive = lifetimeService.KeepAlive;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to handle end of lifetime");
            }
        } while (keepAlive);
    }
}