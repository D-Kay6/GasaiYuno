using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.StickyMessages.Models;
using MediatR;

namespace GasaiYuno.Discord.StickyMessages.Mediator.Requests;

public class ListStickyMessagesRequestHandler : IRequestHandler<ListStickyMessagesRequest, List<StickyMessage>>
{
    private readonly IUnitOfWork<StickyMessage> _unitOfWork;
    
    public ListStickyMessagesRequestHandler(IUnitOfWork<StickyMessage> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Task<List<StickyMessage>> Handle(ListStickyMessagesRequest request, CancellationToken cancellationToken)
    {
        return _unitOfWork.WhereAsync(x => x.Server == request.ServerId && x.Channel == request.ChannelId, cancellationToken);
    }
}