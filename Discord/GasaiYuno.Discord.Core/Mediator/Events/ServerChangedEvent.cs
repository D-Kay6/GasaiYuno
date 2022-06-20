using Discord.WebSocket;
using MediatR;

namespace GasaiYuno.Discord.Core.Mediator.Events;

public record ServerChangedEvent : INotification
{
    public ulong Id { get; }
    public SocketGuild OldGuild { get; }
    public SocketGuild NewGuild { get; }

    public ServerChangedEvent(SocketGuild oldGuild, SocketGuild newGuild)
    {
        Id = newGuild.Id;
        OldGuild = oldGuild;
        NewGuild = newGuild;
    }
}