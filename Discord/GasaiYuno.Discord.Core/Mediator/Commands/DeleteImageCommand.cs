using MediatR;

namespace GasaiYuno.Discord.Core.Mediator.Commands;

public sealed record DeleteImageCommand : INotification
{
    public string Path { get; init; }

    public DeleteImageCommand(string path)
    {
        Path = path;
    }
}