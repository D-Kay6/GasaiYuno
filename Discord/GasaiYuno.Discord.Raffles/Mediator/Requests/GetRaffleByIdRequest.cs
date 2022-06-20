using GasaiYuno.Discord.Raffles.Models;
using MediatR;

namespace GasaiYuno.Discord.Raffles.Mediator.Requests;

public record GetRaffleByIdRequest : IRequest<Raffle>
{
    public ulong Id { get; }
    
    public GetRaffleByIdRequest(ulong id)
    {
        Id = id;
    }
}