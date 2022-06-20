using GasaiYuno.Discord.DynamicChannels.Models;
using MediatR;

namespace GasaiYuno.Discord.DynamicChannels.Mediator.Commands;

public record AddDynamicChannelCommand : INotification
{
    public ulong ServerId { get; }
    public string Name { get; }
    public AutomationType Type { get; }
    
    public AddDynamicChannelCommand(DynamicChannel command)
    {
        ServerId = command.Server;
        Name = command.Name;
        Type = command.Type;
    }
    
    public AddDynamicChannelCommand(ulong serverId, string name, AutomationType type)
    {
        ServerId = serverId;
        Name = name;
        Type = type;
    }
}