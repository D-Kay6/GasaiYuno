using MediatR;

namespace GasaiYuno.Discord.Core.Mediator.Events;

public record ServerJoinedEvent : INotification
{
    public ulong Id { get; }
    public string Name { get; }

    public ServerJoinedEvent(ulong id, string name)
    {
        Id = id;
        Name = name;
    }
}