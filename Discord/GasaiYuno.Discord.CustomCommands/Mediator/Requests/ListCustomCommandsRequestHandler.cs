using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.CustomCommands.Models;
using MediatR;

namespace GasaiYuno.Discord.CustomCommands.Mediator.Requests;

public class ListCustomCommandsRequestHandler : IRequestHandler<ListCustomCommandsRequest, List<CustomCommand>>
{
    private readonly IUnitOfWork<CustomCommand> _unitOfWork;

    public ListCustomCommandsRequestHandler(IUnitOfWork<CustomCommand> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Task<List<CustomCommand>> Handle(ListCustomCommandsRequest request, CancellationToken cancellationToken)
    {
        return _unitOfWork.WhereAsync(x => x.Server == request.ServerId, cancellationToken);
    }
}