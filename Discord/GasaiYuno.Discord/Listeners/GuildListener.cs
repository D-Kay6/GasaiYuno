using Discord.WebSocket;
using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.Core.Mediator.Events;
using MediatR;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Listeners;

internal class GuildListener : IListener
{
    public int Priority => 1;

    private readonly DiscordShardedClient _client;
    private readonly IMediator _mediator;

    public GuildListener(DiscordShardedClient client, IMediator mediator)
    {
        _client = client;
        _mediator = mediator;
    }

    public Task Start()
    {
        _client.GuildUpdated += GuildUpdated;
        _client.JoinedGuild += GuildJoinedAsync;
        _client.LeftGuild += GuildLeftAsync;
        return Task.CompletedTask;
    }

    private Task GuildUpdated(SocketGuild oldGuild, SocketGuild newGuild)
    {
        return _mediator.Publish(new ServerChangedEvent(oldGuild, newGuild));
    }

    private Task GuildJoinedAsync(SocketGuild guild)
    {
        return _mediator.Publish(new ServerJoinedEvent(guild.Id, guild.Name));
    }

    private Task GuildLeftAsync(SocketGuild guild)
    {
        return _mediator.Publish(new ServerLeftEvent(guild.Id));
    }
    
    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}