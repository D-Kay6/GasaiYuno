using GasaiYuno.Discord.AutoChannels.Models;
using MediatR;

namespace GasaiYuno.Discord.AutoChannels.Mediator.Requests;

public record GetAutoChannelRequest : IRequest<AutoChannel>
{
    public ulong ServerId { get; }
    public ulong ChannelId { get; }

    public GetAutoChannelRequest(ulong serverId, ulong channelId)
    {
        ServerId = serverId;
        ChannelId = channelId;
    }
}