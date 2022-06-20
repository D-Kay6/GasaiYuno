using GasaiYuno.Discord.CustomCommands.Models;
using MediatR;

namespace GasaiYuno.Discord.CustomCommands.Mediator.Requests;

public record GetCustomCommandRequest : IRequest<CustomCommand>
{
    public ulong ServerId { get; }
    public string Command { get; }

    public GetCustomCommandRequest(ulong serverId, string command)
    {
        ServerId = serverId;
        Command = command;
    }
}