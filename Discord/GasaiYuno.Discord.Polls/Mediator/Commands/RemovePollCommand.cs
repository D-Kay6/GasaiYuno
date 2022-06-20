using MediatR;

namespace GasaiYuno.Discord.Polls.Mediator.Commands;

public record RemovePollCommand : INotification
{
    public ulong ServerId { get; }
    public ulong ChannelId { get; }
    public ulong MessageId { get; }
    
    public RemovePollCommand(ulong serverId, ulong channelId, ulong messageId)
    {
        ServerId = serverId;
        ChannelId = channelId;
        MessageId = messageId;
    }
}