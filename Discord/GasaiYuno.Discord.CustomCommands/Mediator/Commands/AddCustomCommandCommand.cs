using GasaiYuno.Discord.CustomCommands.Models;
using MediatR;

namespace GasaiYuno.Discord.CustomCommands.Mediator.Commands;

public record AddCustomCommandCommand : INotification
{
    public ulong ServerId { get; }
    public string Command { get; }
    public string Response { get; }
    
    public AddCustomCommandCommand(CustomCommand command)
    {
        ServerId = command.Server;
        Command = command.Command;
        Response = command.Response;
    }
    
    public AddCustomCommandCommand(ulong serverId, string command, string response)
    {
        ServerId = serverId;
        Command = command;
        Response = response;
    }
}