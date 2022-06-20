using GasaiYuno.Discord.DistributionRoles.Models;
using MediatR;

namespace GasaiYuno.Discord.DistributionRoles.Mediator.Requests;

public record GetDistributionRoleRequest : IRequest<DistributionRole>
{
    public ulong ServerId { get; init; }
    public ulong ChannelId { get; init; }
    public ulong MessageId { get; init; }

    public GetDistributionRoleRequest(ulong serverId, ulong channelId, ulong messageId)
    {
        ServerId = serverId;
        ChannelId = channelId;
        MessageId = messageId;
    }
}