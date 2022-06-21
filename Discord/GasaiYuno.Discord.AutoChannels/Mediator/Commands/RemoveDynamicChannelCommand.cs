using GasaiYuno.Discord.AutoChannels.Models;
using MediatR;

namespace GasaiYuno.Discord.AutoChannels.Mediator.Commands;

public record RemoveDynamicChannelCommand : INotification
{
    public ulong ServerId { get; }
    public string Name { get; }
    
    public RemoveDynamicChannelCommand(DynamicChannel dynamicChannel)
    {
        ServerId = dynamicChannel.Server;
        Name = dynamicChannel.Name;
    }
    
    public RemoveDynamicChannelCommand(ulong serverId, string name)
    {
        ServerId = serverId;
        Name = name;
    }
}