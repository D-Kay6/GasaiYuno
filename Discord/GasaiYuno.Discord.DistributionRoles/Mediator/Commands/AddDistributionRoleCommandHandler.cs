using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.DistributionRoles.Models;
using MediatR;

namespace GasaiYuno.Discord.DistributionRoles.Mediator.Commands;

public class AddDistributionRoleCommandHandler : INotificationHandler<AddDistributionRoleCommand>
{
    private readonly IUnitOfWork<DistributionRole> _unitOfWork;
    
    public AddDistributionRoleCommandHandler(IUnitOfWork<DistributionRole> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(AddDistributionRoleCommand command, CancellationToken cancellationToken)
    {
        var distributionRole = await _unitOfWork.SingleOrDefaultAsync(x => x.Server == command.ServerId && x.Channel == command.ChannelId && x.Message == command.MessageId, cancellationToken).ConfigureAwait(false);
        if (distributionRole != null) return;
        
        distributionRole = new DistributionRole
        {
            Server = command.ServerId,
            Channel = command.ChannelId,
            Message = command.MessageId,
            Description = command.Description,
            MinSelected = command.MinSelected,
            MaxSelected = command.MaxSelected,
            ButtonText = command.ButtonText
        };
        await _unitOfWork.AddAsync(distributionRole, cancellationToken).ConfigureAwait(false);
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}