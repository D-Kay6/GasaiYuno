using GasaiYuno.Discord.Raffles.Models;
using MediatR;

namespace GasaiYuno.Discord.Raffles.Mediator.Requests;

public record GetRaffleRequest : IRequest<Raffle>
{
    public ulong ServerId { get; }
    public ulong ChannelId { get; }
    public ulong MessageId { get; }
    
    public GetRaffleRequest(ulong serverId, ulong channelId, ulong messageId)
    {
        ServerId = serverId;
        ChannelId = channelId;
        MessageId = messageId;
    }
}