using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.Core.Models;
using MediatR;

namespace GasaiYuno.Discord.Mediator.Requests;

public class ListServersRequestHandler : IRequestHandler<ListServersRequest, List<Server>>
{
    private readonly IUnitOfWork<Server> _unitOfWork;

    public ListServersRequestHandler(IUnitOfWork<Server> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Task<List<Server>> Handle(ListServersRequest request, CancellationToken cancellationToken)
    {
        return _unitOfWork.GetAllAsync(cancellationToken);
    }
}