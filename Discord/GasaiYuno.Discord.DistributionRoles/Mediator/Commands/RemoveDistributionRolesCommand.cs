using MediatR;

namespace GasaiYuno.Discord.DistributionRoles.Mediator.Commands;

public record RemoveDistributionRolesCommand : INotification
{
    public ulong ServerId { get; }
    public ulong ChannelId { get; }

    public RemoveDistributionRolesCommand(ulong serverId, ulong channelId)
    {
        ServerId = serverId;
        ChannelId = channelId;
    }
}