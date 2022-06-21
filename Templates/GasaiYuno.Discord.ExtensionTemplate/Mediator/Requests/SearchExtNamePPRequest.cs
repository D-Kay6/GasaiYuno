using GasaiYuno.Discord.ExtNamePP.Models;
using MediatR;

namespace GasaiYuno.Discord.ExtNamePP.Mediator.Requests;

public record SearchExtNamePPRequest : IRequest<List<ExtNamePS>>
{
    public string SearchTerm { get; }
    
    public SearchExtNamePPRequest(string searchTerm)
    {
        SearchTerm = searchTerm;
    }
}