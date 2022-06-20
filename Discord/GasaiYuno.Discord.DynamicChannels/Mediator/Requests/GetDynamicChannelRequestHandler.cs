using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.DynamicChannels.Models;
using MediatR;

namespace GasaiYuno.Discord.DynamicChannels.Mediator.Requests;

public class GetDynamicChannelRequestHandler : IRequestHandler<GetDynamicChannelRequest, DynamicChannel>
{
    private readonly IUnitOfWork<DynamicChannel> _unitOfWork;

    public GetDynamicChannelRequestHandler(IUnitOfWork<DynamicChannel> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Task<DynamicChannel> Handle(GetDynamicChannelRequest request, CancellationToken cancellationToken)
    {
        return _unitOfWork.SingleOrDefaultAsync(x => x.Server == request.ServerId && x.Name == request.Name, false, cancellationToken);
    }
}