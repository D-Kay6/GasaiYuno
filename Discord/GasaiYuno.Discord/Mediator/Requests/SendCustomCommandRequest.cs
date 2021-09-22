using Discord.Commands;
using GasaiYuno.Discord.Domain;
using MediatR;
using System.Runtime.Serialization;

namespace GasaiYuno.Discord.Mediator.Requests
{
    public class SendCustomCommandRequest : IRequest<bool>
    {
        public ICommandContext Context { get; init; }
        public Server Server { get; init; }

        public SendCustomCommandRequest(ICommandContext context, Server server)
        {
            Context = context;
            Server = server;
        }
    }
}