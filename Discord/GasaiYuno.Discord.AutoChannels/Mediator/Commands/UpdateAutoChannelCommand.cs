using GasaiYuno.Discord.AutoChannels.Models;
using MediatR;

namespace GasaiYuno.Discord.AutoChannels.Mediator.Commands;

public record UpdateAutoChannelCommand : INotification
{
    public ulong ServerId { get; }
    public ulong ChannelId { get; }
    public AutomationType Type { get; }
    public List<ulong> RelatedChannels { get; }
    public string GenerationName { get; }
    public List<ulong> GeneratedChannels { get; }

    public UpdateAutoChannelCommand(AutoChannel autoChannel)
    {
        ServerId = autoChannel.Server;
        ChannelId = autoChannel.Channel;
        Type = autoChannel.Type;
        RelatedChannels = autoChannel.RelatedChannels;
        GenerationName = autoChannel.GenerationName;
        GeneratedChannels = autoChannel.GeneratedChannels;
    }

    public UpdateAutoChannelCommand(ulong serverId, ulong channelId, AutomationType type, string generationName)
    {
        ServerId = serverId;
        ChannelId = channelId;
        Type = type;
        GenerationName = generationName;
    }

    public UpdateAutoChannelCommand(ulong serverId, ulong channelId, AutomationType type, List<ulong> relatedChannels, string generationName, List<ulong> generatedChannels)
    {
        ServerId = serverId;
        ChannelId = channelId;
        Type = type;
        RelatedChannels = relatedChannels;
        GenerationName = generationName;
        GeneratedChannels = generatedChannels;
    }
}