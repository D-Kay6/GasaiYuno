using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.DistributionRoles.Models;
using MediatR;

namespace GasaiYuno.Discord.DistributionRoles.Mediator.Requests;

public class ListDistributionRolesRequestHandler : IRequestHandler<ListDistributionRolesRequest, List<DistributionRole>>
{
    private readonly IUnitOfWork<DistributionRole> _unitOfWork;

    public ListDistributionRolesRequestHandler(IUnitOfWork<DistributionRole> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Task<List<DistributionRole>> Handle(ListDistributionRolesRequest request, CancellationToken cancellationToken)
    {
        return _unitOfWork.WhereAsync(x => x.Server == request.ServerId && x.Channel == request.ChannelId, cancellationToken);
    }
}