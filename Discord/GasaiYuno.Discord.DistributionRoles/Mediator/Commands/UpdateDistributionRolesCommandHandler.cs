using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.DistributionRoles.Models;
using MediatR;

namespace GasaiYuno.Discord.DistributionRoles.Mediator.Commands;

public class UpdateDistributionRolesCommandHandler : INotificationHandler<UpdateDistributionRolesCommand>
{
    private readonly IUnitOfWork<DistributionRole> _unitOfWork;

    public UpdateDistributionRolesCommandHandler(IUnitOfWork<DistributionRole> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateDistributionRolesCommand command, CancellationToken cancellationToken)
    {
        var distributionRole = await _unitOfWork.SingleOrDefaultAsync(x => x.Server == command.ServerId && x.Channel == command.ChannelId && x.Message == command.MessageId, cancellationToken).ConfigureAwait(false);
        if (distributionRole == null) return;

        var rolesToRemove = distributionRole.Roles.Where(x => x.Value == command.SelectorId).Select(x => x.Key);
        foreach (var role in rolesToRemove)
            distributionRole.Roles.Remove(role);
        foreach (var selectedRole in command.SelectedRoles)
            distributionRole.Roles.Add(selectedRole, command.SelectorId);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}