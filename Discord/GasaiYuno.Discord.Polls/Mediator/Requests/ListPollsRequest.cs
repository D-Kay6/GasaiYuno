using GasaiYuno.Discord.Polls.Models;
using MediatR;

namespace GasaiYuno.Discord.Polls.Mediator.Requests;

public record ListPollsRequest : IRequest<List<Poll>>
{
    public bool ExpiredOnly { get; }
    
    public ListPollsRequest(bool expiredOnly)
    {
        ExpiredOnly = expiredOnly;
    }
}