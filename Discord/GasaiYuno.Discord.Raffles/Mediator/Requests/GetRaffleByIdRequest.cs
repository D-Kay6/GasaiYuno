using GasaiYuno.Discord.Raffles.Models;
using MediatR;

namespace GasaiYuno.Discord.Raffles.Mediator.Requests;

public record GetRaffleByIdRequest : IRequest<Raffle>
{
    public Guid Id { get; }
    
    public GetRaffleByIdRequest(Guid id)
    {
        Id = id;
    }
}