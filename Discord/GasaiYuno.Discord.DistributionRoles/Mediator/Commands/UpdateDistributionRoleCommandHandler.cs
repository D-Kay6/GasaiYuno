using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.DistributionRoles.Models;
using MediatR;

namespace GasaiYuno.Discord.DistributionRoles.Mediator.Commands;

public class UpdateDistributionRoleCommandHandler : INotificationHandler<UpdateDistributionRoleCommand>
{
    private readonly IUnitOfWork<DistributionRole> _unitOfWork;

    public UpdateDistributionRoleCommandHandler(IUnitOfWork<DistributionRole> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateDistributionRoleCommand command, CancellationToken cancellationToken)
    {
        var distributionRole = await _unitOfWork.SingleOrDefaultAsync(x => x.Server == command.ServerId && x.Channel == command.ChannelId && x.Message == command.MessageId, cancellationToken).ConfigureAwait(false);
        if (distributionRole == null) return;

        distributionRole.Message = command.NewMessageId;
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}