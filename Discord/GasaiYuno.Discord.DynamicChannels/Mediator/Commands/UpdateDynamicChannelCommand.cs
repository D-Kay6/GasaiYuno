using GasaiYuno.Discord.DynamicChannels.Models;
using MediatR;

namespace GasaiYuno.Discord.DynamicChannels.Mediator.Commands;

public record UpdateDynamicChannelCommand : INotification
{
    public ulong ServerId { get; }
    public string Name { get; }
    public AutomationType Type { get; }
    public string GenerationName { get; }
    public List<ulong> Channels { get; }
    public List<ulong> GeneratedChannels { get; }

    public UpdateDynamicChannelCommand(DynamicChannel dynamicChannel)
    {
        ServerId = dynamicChannel.Server;
        Name = dynamicChannel.Name;
        Type = dynamicChannel.Type;
        GenerationName = dynamicChannel.GenerationName;
        Channels = dynamicChannel.Channels;
        GeneratedChannels = dynamicChannel.GeneratedChannels;
    }

    public UpdateDynamicChannelCommand(ulong serverId, string name, AutomationType type, string generationName, List<ulong> channels, List<ulong> generatedChannels)
    {
        ServerId = serverId;
        Name = name;
        Type = type;
        GenerationName = generationName;
        Channels = channels;
        GeneratedChannels = generatedChannels;
    }
}