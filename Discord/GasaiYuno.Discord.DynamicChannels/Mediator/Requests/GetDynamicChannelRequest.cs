using GasaiYuno.Discord.DynamicChannels.Models;
using MediatR;

namespace GasaiYuno.Discord.DynamicChannels.Mediator.Requests;

public record GetDynamicChannelRequest : IRequest<DynamicChannel>
{
    public ulong ServerId { get; }
    public string Name { get; }

    public GetDynamicChannelRequest(ulong serverId, string name)
    {
        ServerId = serverId;
        Name = name;
    }
}