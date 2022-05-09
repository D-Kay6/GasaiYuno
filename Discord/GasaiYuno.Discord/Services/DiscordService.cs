using Autofac;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

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
                await using var scope = _lifetimeScope.BeginLifetimeScope("DiscordLifetime");
                var lifetimeService = scope.Resolve<LifetimeService>();
                await lifetimeService.StartAsync(cancellationToken);
                keepAlive = lifetimeService.KeepAlive;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to handle end of lifetime");
            }
        } while (keepAlive);
    }
}