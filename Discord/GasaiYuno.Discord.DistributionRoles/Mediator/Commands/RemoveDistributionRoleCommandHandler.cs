using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.DistributionRoles.Models;
using MediatR;

namespace GasaiYuno.Discord.DistributionRoles.Mediator.Commands;

public class DeleteDistributionRoleCommandHandler : INotificationHandler<RemoveDistributionRoleCommand>
{
    private readonly IUnitOfWork<DistributionRole> _unitOfWork;

    public DeleteDistributionRoleCommandHandler(IUnitOfWork<DistributionRole> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(RemoveDistributionRoleCommand command, CancellationToken cancellationToken)
    {
        var distributionRole = await _unitOfWork.SingleOrDefaultAsync(x => x.Server == command.ServerId && x.Channel == command.ChannelId && x.Message == command.MessageId, cancellationToken).ConfigureAwait(false);
        if (distributionRole == null) return;

        _unitOfWork.Remove(distributionRole);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}