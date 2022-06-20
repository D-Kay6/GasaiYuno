using Discord.WebSocket;
using GasaiYuno.Discord.Bans.Mediator.Commands;
using GasaiYuno.Discord.Bans.Mediator.Requests;
using GasaiYuno.Discord.Core.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GasaiYuno.Discord.Bans.Listeners;

internal class BanListener : IListener
{
    public int Priority => 1;
    
    private readonly DiscordShardedClient _client;
    private readonly IMediator _mediator;
    private readonly ILogger<BanListener> _logger;
    private readonly Timer _timer;

    public BanListener(DiscordShardedClient client, IMediator mediator, ILogger<BanListener> logger)
    {
        _client = client;
        _mediator = mediator;
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
        var bans = await _mediator.Send(new ListBansRequest(expiredOnly: true)).ConfigureAwait(false);
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
            
            await _mediator.Publish(new RemoveBanCommand(ban.Server, ban.User)).ConfigureAwait(false);
        }

        _timer.Change(TimeSpan.FromMinutes(10), Timeout.InfiniteTimeSpan);
    }

    public ValueTask DisposeAsync()
    {
        return _timer?.DisposeAsync() ?? ValueTask.CompletedTask;
    }
}