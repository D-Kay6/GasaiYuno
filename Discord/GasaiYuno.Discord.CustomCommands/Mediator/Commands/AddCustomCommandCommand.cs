using GasaiYuno.Discord.CustomCommands.Models;
using MediatR;

namespace GasaiYuno.Discord.CustomCommands.Mediator.Commands;

public record AddCustomCommandCommand : INotification
{
    public ulong ServerId { get; }
    public string Command { get; }
    public string Response { get; }
    
    public AddCustomCommandCommand(CustomCommand customCommand)
    {
        ServerId = customCommand.Server;
        Command = customCommand.Command;
        Response = customCommand.Response;
    }
    
    public AddCustomCommandCommand(ulong serverId, string command, string response)
    {
        ServerId = serverId;
        Command = command;
        Response = response;
    }
}