using GasaiYuno.Discord.DistributionRoles.Models;
using MediatR;

namespace GasaiYuno.Discord.DistributionRoles.Mediator.Requests;

public record ListDistributionRolesRequest : IRequest<List<DistributionRole>>
{
    public ulong ServerId { get; init; }
    public ulong ChannelId { get; init; }

    public ListDistributionRolesRequest(ulong serverId, ulong channelId)
    {
        ServerId = serverId;
        ChannelId = channelId;
    }
}