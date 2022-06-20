using GasaiYuno.Discord.CustomCommands.Models;
using MediatR;

namespace GasaiYuno.Discord.CustomCommands.Mediator.Requests;

public record SearchCustomCommandsRequest : IRequest<List<CustomCommand>>
{
    public ulong ServerId { get; }
    public string SearchTerm { get; }
    
    public SearchCustomCommandsRequest(ulong serverId, string searchTerm)
    {
        ServerId = serverId;
        SearchTerm = searchTerm;
    }
}