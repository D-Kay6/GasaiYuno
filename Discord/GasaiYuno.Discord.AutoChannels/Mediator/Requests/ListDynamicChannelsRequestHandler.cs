using GasaiYuno.Discord.AutoChannels.Models;
using GasaiYuno.Discord.Core.Interfaces;
using MediatR;

namespace GasaiYuno.Discord.AutoChannels.Mediator.Requests;

public class ListDynamicChannelsRequestHandler : IRequestHandler<ListDynamicChannelsRequest, List<DynamicChannel>>
{
    private readonly IUnitOfWork<DynamicChannel> _unitOfWork;

    public ListDynamicChannelsRequestHandler(IUnitOfWork<DynamicChannel> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Task<List<DynamicChannel>> Handle(ListDynamicChannelsRequest request, CancellationToken cancellationToken)
    {
        return _unitOfWork.GetAllAsync(cancellationToken);
    }
}