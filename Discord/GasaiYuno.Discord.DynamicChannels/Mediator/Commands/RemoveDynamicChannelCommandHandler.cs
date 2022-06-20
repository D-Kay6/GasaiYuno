using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.DynamicChannels.Models;
using MediatR;

namespace GasaiYuno.Discord.DynamicChannels.Mediator.Commands;

public class RemoveDynamicChannelCommandHandler : INotificationHandler<RemoveDynamicChannelCommand>
{
    private readonly IUnitOfWork<DynamicChannel> _unitOfWork;
    
    public RemoveDynamicChannelCommandHandler(IUnitOfWork<DynamicChannel> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(RemoveDynamicChannelCommand command, CancellationToken cancellationToken)
    {
        var dynamicChannel = await _unitOfWork.SingleOrDefaultAsync(x => x.Server == command.ServerId && x.Name == command.Name, cancellationToken).ConfigureAwait(false);
        if (dynamicChannel == null) return;
        
        _unitOfWork.Remove(dynamicChannel);
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}