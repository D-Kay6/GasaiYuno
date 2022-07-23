using Discord.WebSocket;
using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.Listing.Mediator.Events;
using MediatR;

namespace GasaiYuno.Discord.Listing.Listeners;

internal class ListingListener : IListener
{
    public int Priority => 1;

    private readonly DiscordShardedClient _client;
    private readonly IMediator _mediator;

    public ListingListener(DiscordShardedClient client, IMediator mediator)
    {
        _client = client;
        _mediator = mediator;
    }

    public Task Start()
    {
#if DEBUG
        return Task.CompletedTask;
#endif
        _client.JoinedGuild += OnJoinedGuild;
        _client.LeftGuild += OnLeftGuild;
        return _mediator.Publish(new GuildCountChangedEvent(_client.CurrentUser.Id, _client.Shards.Count, _client.Guilds.Count));
    }

    private Task OnJoinedGuild(SocketGuild _)
    {
        return _mediator.Publish(new GuildCountChangedEvent(_client.CurrentUser.Id, _client.Shards.Count, _client.Guilds.Count));
    }

    private Task OnLeftGuild(SocketGuild _)
    {
        return _mediator.Publish(new GuildCountChangedEvent(_client.CurrentUser.Id, _client.Shards.Count, _client.Guilds.Count));
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}