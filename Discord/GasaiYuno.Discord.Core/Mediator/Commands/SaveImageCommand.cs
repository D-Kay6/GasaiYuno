using MediatR;

namespace GasaiYuno.Discord.Core.Mediator.Commands
{
    public sealed record SaveImageCommand : IRequest<string>
    {
        public string Url { get; init; }
        public string Directory { get; init; }

        public SaveImageCommand(string url, string directory)
        {
            Url = url;
            Directory = directory;
        }
    }
}