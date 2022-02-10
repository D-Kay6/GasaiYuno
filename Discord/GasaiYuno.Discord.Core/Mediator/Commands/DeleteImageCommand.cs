using MediatR;

namespace GasaiYuno.Discord.Core.Mediator.Commands
{
    public sealed record DeleteImageCommand : IRequest
    {
        public string Path { get; init; }

        public DeleteImageCommand(string path)
        {
            Path = path;
        }
    }
}