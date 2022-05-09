using Discord.WebSocket;
using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.Domain.Persistence.UnitOfWork;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Listeners;

internal class BanListener : IListener
{
    public int Priority => 1;
    
    private readonly DiscordShardedClient _client;
    private readonly Func<IUnitOfWork> _unitOfWorkFactory;
    private readonly ILogger<BanListener> _logger;
    private readonly Timer _timer;

    public BanListener(DiscordShardedClient client, Func<IUnitOfWork> unitOfWorkFactory, ILogger<BanListener> logger)
    {
        _client = client;
        _unitOfWorkFactory = unitOfWorkFactory;
        _logger = logger;
        _timer = new Timer(CheckBans, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
    }

    public Task Start()
    {
        _timer.Change(TimeSpan.FromMinutes(10), Timeout.InfiniteTimeSpan);
        return Task.CompletedTask;
    }

    private async void CheckBans(object stateInfo)
    {
        var unitOfWork = _unitOfWorkFactory();
        var bans = await unitOfWork.Bans.ListAsync(expired: true).ConfigureAwait(false);
        foreach (var ban in bans)
        {
            var server = _client.GetGuild(ban.Server);
            if (server == null) continue;

            var serverBan = await server.GetBanAsync(ban.User).ConfigureAwait(false);
            if (serverBan != null)
            {
                try
                {
                    await server.RemoveBanAsync(ban.User).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Unable to remove ban {@Ban} from server {ServerName}({ServerId})", ban, server.Name, server.Id);
                }
            }
                
            unitOfWork.Bans.Remove(ban);
            await unitOfWork.SaveChangesAsync().ConfigureAwait(false);
        }

        _timer.Change(TimeSpan.FromMinutes(10), Timeout.InfiniteTimeSpan);
    }

    public ValueTask DisposeAsync()
    {
        return _timer?.DisposeAsync() ?? ValueTask.CompletedTask;
    }
}