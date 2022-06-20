using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.DynamicChannels.Models;
using MediatR;

namespace GasaiYuno.Discord.DynamicChannels.Mediator.Requests;

public class SearchDynamicChannelsRequestHandler : IRequestHandler<SearchDynamicChannelsRequest, List<DynamicChannel>>
{
    private readonly IUnitOfWork<DynamicChannel> _unitOfWork;
    private readonly IMediator _mediator;
    
    public SearchDynamicChannelsRequestHandler(IUnitOfWork<DynamicChannel> unitOfWork, IMediator mediator)
    {
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    public Task<List<DynamicChannel>> Handle(SearchDynamicChannelsRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.SearchTerm))
            return _mediator.Send(new ListDynamicChannelsRequest(request.ServerId), cancellationToken);
        
        return _unitOfWork.SearchAsync(x => x.Server == request.ServerId, x => x.Name, $"*{request.SearchTerm}*", cancellationToken);
    }
}