using GasaiYuno.Discord.DynamicChannels.Models;
using MediatR;

namespace GasaiYuno.Discord.DynamicChannels.Mediator.Requests;

public record ListDynamicChannelsRequest : IRequest<List<DynamicChannel>>
{
    public ulong ServerId { get; }
    public AutomationType? Type { get; }

    public ListDynamicChannelsRequest(ulong serverId, AutomationType? type = null)
    {
        ServerId = serverId;
        Type = type;
    }
}