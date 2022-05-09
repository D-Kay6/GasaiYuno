using Discord.WebSocket;
using MediatR;

namespace GasaiYuno.Discord.Core.Mediator.Events;

public sealed record ClientReadyEvent : INotification
{
    public DiscordShardedClient Client { get; init; }

    public ClientReadyEvent(DiscordShardedClient client)
    {
        Client = client;
    }
}