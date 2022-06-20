using GasaiYuno.Discord.StickyMessages.Models;
using MediatR;

namespace GasaiYuno.Discord.StickyMessages.Mediator.Requests;

public record ListStickyMessagesRequest : IRequest<List<StickyMessage>>
{
    public ulong ServerId { get; }
    public ulong ChannelId { get; }
    
    public ListStickyMessagesRequest(ulong serverId, ulong channelId)
    {
        ServerId = serverId;
        ChannelId = channelId;
    }
}