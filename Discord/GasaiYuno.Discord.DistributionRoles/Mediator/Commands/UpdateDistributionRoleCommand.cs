using MediatR;

namespace GasaiYuno.Discord.DistributionRoles.Mediator.Commands;

public record UpdateDistributionRoleCommand : INotification
{
    public ulong ServerId { get; }
    public ulong ChannelId { get; }
    public ulong MessageId { get; }
    public ulong NewMessageId { get; }
    
    public UpdateDistributionRoleCommand(ulong serverId, ulong channelId, ulong messageId, ulong newMessageId)
    {
        ServerId = serverId;
        ChannelId = channelId;
        MessageId = messageId;
        NewMessageId = newMessageId;
    }
}