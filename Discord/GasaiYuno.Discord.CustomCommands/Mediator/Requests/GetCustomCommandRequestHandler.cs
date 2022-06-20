using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.CustomCommands.Models;
using MediatR;

namespace GasaiYuno.Discord.CustomCommands.Mediator.Requests;

public class GetCustomCommandRequestHandler : IRequestHandler<GetCustomCommandRequest, CustomCommand>
{
    private readonly IUnitOfWork<CustomCommand> _unitOfWork;

    public GetCustomCommandRequestHandler(IUnitOfWork<CustomCommand> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Task<CustomCommand> Handle(GetCustomCommandRequest request, CancellationToken cancellationToken)
    {
        return _unitOfWork.SingleOrDefaultAsync(x => x.Server == request.ServerId && x.Command == request.Command, false, cancellationToken);
    }
}