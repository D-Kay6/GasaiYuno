using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.Polls.Models;
using MediatR;

namespace GasaiYuno.Discord.Polls.Mediator.Requests;

public class GetPollRequestHandler : IRequestHandler<GetPollRequest, Poll>
{
    private readonly IUnitOfWork<Poll> _unitOfWork;
    
    public GetPollRequestHandler(IUnitOfWork<Poll> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Task<Poll> Handle(GetPollRequest request, CancellationToken cancellationToken)
    {
        return _unitOfWork.SingleOrDefaultAsync(x => x.Server == request.ServerId && x.Channel == request.ChannelId && x.Message == request.MessageId, cancellationToken);
    }
}