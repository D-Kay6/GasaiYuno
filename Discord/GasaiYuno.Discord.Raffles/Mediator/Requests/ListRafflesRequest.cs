using GasaiYuno.Discord.Raffles.Models;
using MediatR;

namespace GasaiYuno.Discord.Raffles.Mediator.Requests;

public record ListRafflesRequest : IRequest<List<Raffle>>
{
    public bool ExpiredOnly { get; }
    
    public ListRafflesRequest(bool expiredOnly)
    {
        ExpiredOnly = expiredOnly;
    }
}