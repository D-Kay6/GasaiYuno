using GasaiYuno.Discord.AutoChannels.Models;
using MediatR;

namespace GasaiYuno.Discord.AutoChannels.Mediator.Commands;

public record RemoveAutoChannelCommand : INotification
{
    public ulong ServerId { get; }
    public ulong ChannelId { get; }
    
    public RemoveAutoChannelCommand(AutoChannel autoChannel)
    {
        ServerId = autoChannel.Server;
        ChannelId = autoChannel.Channel;
    }
    
    public RemoveAutoChannelCommand(ulong serverId, ulong channelId)
    {
        ServerId = serverId;
        ChannelId = channelId;
    }
}