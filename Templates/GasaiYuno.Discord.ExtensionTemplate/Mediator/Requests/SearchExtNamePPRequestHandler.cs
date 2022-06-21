using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.ExtNamePP.Models;
using MediatR;

namespace GasaiYuno.Discord.ExtNamePP.Mediator.Requests;

public class SearchExtNamePPRequestHandler : IRequestHandler<SearchExtNamePPRequest, List<ExtNamePS>>
{
    private readonly IUnitOfWork<ExtNamePS> _unitOfWork;
    private readonly IMediator _mediator;
    
    public SearchExtNamePPRequestHandler(IUnitOfWork<ExtNamePS> unitOfWork, IMediator mediator)
    {
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    public Task<List<ExtNamePS>> Handle(SearchExtNamePPRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.SearchTerm))
            return _mediator.Send(new ListExtNamePPRequest(), cancellationToken);
        
        return _unitOfWork.SearchAsync($"*{request.SearchTerm}*", cancellationToken);
    }
}