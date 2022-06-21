using GasaiYuno.Discord.ExtNamePP.Models;
using MediatR;

namespace GasaiYuno.Discord.ExtNamePP.Mediator.Requests;

public record GetExtNamePSRequest : IRequest<ExtNamePS>
{
    public GetExtNamePSRequest()
    {
    }
}