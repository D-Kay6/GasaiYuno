using GasaiYuno.Discord.Domain;
using MediatR;
using System.Runtime.Serialization;

namespace GasaiYuno.Discord.Mediator.Requests
{
    public class GetServerRequest : IRequest<Server>
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