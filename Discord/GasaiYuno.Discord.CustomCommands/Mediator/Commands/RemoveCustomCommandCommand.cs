using GasaiYuno.Discord.CustomCommands.Models;
using MediatR;

namespace GasaiYuno.Discord.CustomCommands.Mediator.Commands;

public record RemoveCustomCommandCommand : INotification
{
    public ulong ServerId { get; }
    public string Command { get; }
    
    public RemoveCustomCommandCommand(CustomCommand customCommand)
    {
        ServerId = customCommand.Server;
        Command = customCommand.Command;
    }
    
    public RemoveCustomCommandCommand(ulong serverId, string command)
    {
        ServerId = serverId;
        Command = command;
    }
}