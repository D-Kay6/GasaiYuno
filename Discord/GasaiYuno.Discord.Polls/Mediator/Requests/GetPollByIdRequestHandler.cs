using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.Polls.Models;
using MediatR;

namespace GasaiYuno.Discord.Polls.Mediator.Requests;

public class GetPollByIdRequestHandler : IRequestHandler<GetPollByIdRequest, Poll>
{
    private readonly IUnitOfWork<Poll> _unitOfWork;
    
    public GetPollByIdRequestHandler(IUnitOfWork<Poll> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Task<Poll> Handle(GetPollByIdRequest request, CancellationToken cancellationToken)
    {
        return _unitOfWork.SingleOrDefaultAsync(x => x.Identity == request.Id, cancellationToken);
    }
}