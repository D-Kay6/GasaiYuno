using MediatR;

namespace GasaiYuno.Discord.Mediator.Commands;

internal sealed record AddServerCommand : INotification
{
    public ulong Id { get; }
    public string Name { get; }

    public AddServerCommand(ulong id, string name)
    {
        Id = id;
        Name = name;
    }
}