using GasaiYuno.Discord.AutoChannels.Models;
using MediatR;

namespace GasaiYuno.Discord.AutoChannels.Mediator.Requests;

public record ListAutoChannelsRequest : IRequest<List<AutoChannel>>
{
    public ulong ServerId { get; }
    public AutomationType? Type { get; }

    public ListAutoChannelsRequest(ulong serverId, AutomationType? type = null)
    {
        ServerId = serverId;
        Type = type;
    }
}