using GasaiYuno.Discord.AutoChannels.Models;
using MediatR;

namespace GasaiYuno.Discord.AutoChannels.Mediator.Commands;

public record AddAutoChannelCommand : INotification
{
    public ulong ServerId { get; }
    public ulong ChannelId { get; }
    public AutomationType Type { get; }
    public string GenerationName { get; }
    
    public AddAutoChannelCommand(AutoChannel autoChannel)
    {
        ServerId = autoChannel.Server;
        ChannelId = autoChannel.Channel;
        Type = autoChannel.Type;
        GenerationName = autoChannel.GenerationName;
    }

    public AddAutoChannelCommand(ulong serverId, ulong channelId, AutomationType type, string generationName)
    {
        ServerId = serverId;
        ChannelId = channelId;
        Type = type;
        GenerationName = generationName;
    }
}