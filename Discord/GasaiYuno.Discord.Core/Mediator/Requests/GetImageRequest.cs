using MediatR;

namespace GasaiYuno.Discord.Core.Mediator.Requests;

public sealed record GetImageRequest : IRequest<string>
{
    public string Name { get; init; }
    public string Directory { get; init; }

    public GetImageRequest(string name, string directory = null)
    {
        Name = name;
        Directory = directory;
    }
}