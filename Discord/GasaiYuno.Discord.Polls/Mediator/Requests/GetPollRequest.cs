using GasaiYuno.Discord.Polls.Models;
using MediatR;

namespace GasaiYuno.Discord.Polls.Mediator.Requests;

public record GetPollRequest : IRequest<Poll>
{
    public ulong ServerId { get; }
    public ulong ChannelId { get; }
    public ulong MessageId { get; }
    
    public GetPollRequest(ulong serverId, ulong channelId, ulong messageId)
    {
        ServerId = serverId;
        ChannelId = channelId;
        MessageId = messageId;
    }
}