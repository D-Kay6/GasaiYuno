using GasaiYuno.Discord.Polls.Models;
using MediatR;

namespace GasaiYuno.Discord.Polls.Mediator.Requests;

public record GetPollByIdRequest : IRequest<Poll>
{
    public ulong Id { get; }
    
    public GetPollByIdRequest(ulong id)
    {
        Id = id;
    }
}