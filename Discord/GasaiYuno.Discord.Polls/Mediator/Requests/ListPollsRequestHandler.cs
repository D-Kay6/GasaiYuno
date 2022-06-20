using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.Polls.Models;
using MediatR;

namespace GasaiYuno.Discord.Polls.Mediator.Requests;

public class ListPollsRequestHandler : IRequestHandler<ListPollsRequest, List<Poll>>
{
    private readonly IUnitOfWork<Poll> _unitOfWork;
    
    public ListPollsRequestHandler(IUnitOfWork<Poll> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Task<List<Poll>> Handle(ListPollsRequest request, CancellationToken cancellationToken)
    {
        if (request.ExpiredOnly)
            return _unitOfWork.WhereAsync(x => x.EndDate < DateTime.Now, cancellationToken);

        return _unitOfWork.GetAllAsync(cancellationToken);
    }
}