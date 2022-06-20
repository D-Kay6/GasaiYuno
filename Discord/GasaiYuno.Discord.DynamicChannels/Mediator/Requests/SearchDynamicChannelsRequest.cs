using GasaiYuno.Discord.DynamicChannels.Models;
using MediatR;

namespace GasaiYuno.Discord.DynamicChannels.Mediator.Requests;

public record SearchDynamicChannelsRequest : IRequest<List<DynamicChannel>>
{
    public ulong ServerId { get; }
    public string SearchTerm { get; }
    
    public SearchDynamicChannelsRequest(ulong serverId, string searchTerm)
    {
        ServerId = serverId;
        SearchTerm = searchTerm;
    }
}