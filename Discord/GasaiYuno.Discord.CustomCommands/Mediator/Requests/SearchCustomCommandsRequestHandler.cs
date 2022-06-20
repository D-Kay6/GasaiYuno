using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.CustomCommands.Models;
using MediatR;

namespace GasaiYuno.Discord.CustomCommands.Mediator.Requests;

public class SearchCustomCommandsRequestHandler : IRequestHandler<SearchCustomCommandsRequest, List<CustomCommand>>
{
    private readonly IUnitOfWork<CustomCommand> _unitOfWork;
    private readonly IMediator _mediator;
    
    public SearchCustomCommandsRequestHandler(IUnitOfWork<CustomCommand> unitOfWork, IMediator mediator)
    {
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    public Task<List<CustomCommand>> Handle(SearchCustomCommandsRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.SearchTerm))
            return _mediator.Send(new ListCustomCommandsRequest(request.ServerId), cancellationToken);
        
        return _unitOfWork.SearchAsync(x => x.Server == request.ServerId, x => x.Command, $"*{request.SearchTerm}*", cancellationToken);
    }
}