using GasaiYuno.Discord.Domain;
using MediatR;

namespace GasaiYuno.Discord.Core.Mediator.Requests
{
    public record GetServerRequest : IRequest<Server>
    {
        public ulong Id { get; init; }
        public string Name { get; init; }

        public GetServerRequest(ulong id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}