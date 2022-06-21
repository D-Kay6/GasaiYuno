using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.AutoChannels.Models;
using MediatR;

namespace GasaiYuno.Discord.AutoChannels.Mediator.Requests;

public class GetAutoChannelRequestHandler : IRequestHandler<GetAutoChannelRequest, AutoChannel>
{
    private readonly IUnitOfWork<AutoChannel> _unitOfWork;

    public GetAutoChannelRequestHandler(IUnitOfWork<AutoChannel> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Task<AutoChannel> Handle(GetAutoChannelRequest request, CancellationToken cancellationToken)
    {
        return _unitOfWork.SingleOrDefaultAsync(x => x.Server == request.ServerId && x.Channel == request.ChannelId, cancellationToken);
    }
}