using GasaiYuno.Discord.CustomCommands.Models;
using MediatR;

namespace GasaiYuno.Discord.CustomCommands.Mediator.Requests;

public record ListCustomCommandsRequest : IRequest<List<CustomCommand>>
{
    public ulong ServerId { get; }

    public ListCustomCommandsRequest(ulong serverId)
    {
        ServerId = serverId;
    }
}