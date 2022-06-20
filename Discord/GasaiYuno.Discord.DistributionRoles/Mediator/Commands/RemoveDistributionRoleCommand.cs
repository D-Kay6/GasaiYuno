using MediatR;

namespace GasaiYuno.Discord.DistributionRoles.Mediator.Commands;

public record RemoveDistributionRoleCommand : INotification
{
    public ulong ServerId { get; }
    public ulong ChannelId { get; }
    public ulong MessageId { get; }

    public RemoveDistributionRoleCommand(ulong serverId, ulong channelId, ulong messageId)
    {
        ServerId = serverId;
        ChannelId = channelId;
        MessageId = messageId;
    }
}