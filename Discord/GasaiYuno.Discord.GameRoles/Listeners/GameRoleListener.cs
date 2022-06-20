using Discord.WebSocket;
using GasaiYuno.Discord.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace GasaiYuno.Discord.GameRoles.Listeners;

public class GameRoleListener : IListener
{
    public int Priority => 1;

    private readonly DiscordShardedClient _client;
    private readonly ILogger<GameRoleListener> _logger;

    public GameRoleListener(DiscordShardedClient client, ILogger<GameRoleListener> logger)
    {
        _client = client;
        _logger = logger;
    }

    public Task Start()
    {
        return Task.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}