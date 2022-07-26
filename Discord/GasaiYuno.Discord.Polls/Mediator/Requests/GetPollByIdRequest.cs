using GasaiYuno.Discord.Polls.Models;
using MediatR;

namespace GasaiYuno.Discord.Polls.Mediator.Requests;

public record GetPollByIdRequest : IRequest<Poll>
{
    public Guid Id { get; }
    
    public GetPollByIdRequest(Guid id)
    {
        Id = id;
    }
}