using GasaiYuno.Discord.DynamicChannels.Models;
using MediatR;

namespace GasaiYuno.Discord.DynamicChannels.Mediator.Commands;

public record RemoveDynamicChannelCommand : INotification
{
    public ulong ServerId { get; }
    public string Name { get; }
    
    public RemoveDynamicChannelCommand(DynamicChannel command)
    {
        ServerId = command.Server;
        Name = command.Name;
    }
    
    public RemoveDynamicChannelCommand(ulong serverId, string name)
    {
        ServerId = serverId;
        Name = name;
    }
}