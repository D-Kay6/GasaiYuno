using GasaiYuno.Discord.ExtNamePP.Models;
using MediatR;

namespace GasaiYuno.Discord.ExtNamePP.Mediator.Requests;

public record ListExtNamePPRequest : IRequest<List<ExtNamePS>>
{
    public ListExtNamePPRequest()
    {
    }
}