using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.DistributionRoles.Models;
using MediatR;

namespace GasaiYuno.Discord.DistributionRoles.Mediator.Commands;

public class RemoveDistributionRolesCommandHandler : INotificationHandler<RemoveDistributionRolesCommand>
{
    private readonly IUnitOfWork<DistributionRole> _unitOfWork;

    public RemoveDistributionRolesCommandHandler(IUnitOfWork<DistributionRole> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(RemoveDistributionRolesCommand command, CancellationToken cancellationToken)
    {
        var distributionRoles = await _unitOfWork.WhereAsync(x => x.Server == command.ServerId && x.Channel == command.ChannelId, cancellationToken).ConfigureAwait(false);
        if (distributionRoles == null || !distributionRoles.Any()) return;

        _unitOfWork.RemoveRange(distributionRoles);
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}