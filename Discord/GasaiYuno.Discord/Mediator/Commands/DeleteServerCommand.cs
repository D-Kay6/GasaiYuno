using MediatR;

namespace GasaiYuno.Discord.Mediator.Commands;

public record DeleteServerCommand : INotification
{
    public ulong Id { get; }

    public DeleteServerCommand(ulong id)
    {
        Id = id;
    }
}