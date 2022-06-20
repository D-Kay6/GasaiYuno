using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.DistributionRoles.Models;
using MediatR;

namespace GasaiYuno.Discord.DistributionRoles.Mediator.Requests;

public class GetDistributionRoleRequestHandler : IRequestHandler<GetDistributionRoleRequest, DistributionRole>
{
    private readonly IUnitOfWork<DistributionRole> _unitOfWork;

    public GetDistributionRoleRequestHandler(IUnitOfWork<DistributionRole> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Task<DistributionRole> Handle(GetDistributionRoleRequest request, CancellationToken cancellationToken)
    {
        return _unitOfWork.SingleOrDefaultAsync(x => x.Server == request.ServerId && x.Channel == request.ChannelId && x.Message == request.MessageId, cancellationToken);
    }
}