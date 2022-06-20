using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.Notifications.Models;
using MediatR;

namespace GasaiYuno.Discord.Notifications.Mediator.Commands;

public class RemoveNotificationCommandHandler : INotificationHandler<RemoveNotificationCommand>
{
    private readonly IUnitOfWork<Notification> _unitOfWork;

    public RemoveNotificationCommandHandler(IUnitOfWork<Notification> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task Handle(RemoveNotificationCommand command, CancellationToken cancellationToken)
    {
        var notification = await _unitOfWork.SingleOrDefaultAsync(x => x.Server == command.ServerId, cancellationToken).ConfigureAwait(false);
        if (notification == null) return;
        
        _unitOfWork.Remove(notification);
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}