using MediatR;

namespace GasaiYuno.Discord.Raffles.Mediator.Commands;

public record RemoveRaffleCommand : INotification
{
    public ulong ServerId { get; }
    public ulong ChannelId { get; }
    public ulong MessageId { get; }
    
    public RemoveRaffleCommand(ulong serverId, ulong channelId, ulong messageId)
    {
        ServerId = serverId;
        ChannelId = channelId;
        MessageId = messageId;
    }
}