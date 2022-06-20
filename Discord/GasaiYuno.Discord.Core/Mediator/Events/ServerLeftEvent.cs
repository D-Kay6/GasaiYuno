using MediatR;

namespace GasaiYuno.Discord.Core.Mediator.Events;

public record ServerLeftEvent : INotification
{
    public ulong Id { get; }

    public ServerLeftEvent(ulong id)
    {
        Id = id;
    }
}